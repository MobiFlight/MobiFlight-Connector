using MobiFlight.SimConnectMSFS.Native;
using System.Runtime.InteropServices;

namespace MobiFlight.SimConnectMSFS.Tests
{
    [TestClass()]
    public class SimConnectDispatchTests
    {
        [TestMethod()]
        public void SystemState_UnmarshalsRequestIdAndString()
        {
            var expected = new SIMCONNECT_RECV_SYSTEM_STATE
            {
                dwSize = (uint)Marshal.SizeOf<SIMCONNECT_RECV_SYSTEM_STATE>(),
                dwVersion = 0,
                dwID = (uint)SIMCONNECT_RECV_ID.SYSTEM_STATE,
                dwRequestID = 42,
                dwInteger = 7,
                fFloat = 1.5f,
                szString = @"C:\Users\Pilot\AppData\aircraft.cfg"
            };

            var actual = RoundTrip(expected);

            Assert.AreEqual(expected.dwRequestID, actual.dwRequestID);
            Assert.AreEqual(expected.dwInteger, actual.dwInteger);
            Assert.AreEqual(expected.fFloat, actual.fFloat);
            Assert.AreEqual(expected.szString, actual.szString);
        }

        [TestMethod()]
        public void EventFilename_UnmarshalsEventIdAndFileName()
        {
            var expected = new SIMCONNECT_RECV_EVENT_FILENAME
            {
                dwSize = (uint)Marshal.SizeOf<SIMCONNECT_RECV_EVENT_FILENAME>(),
                dwVersion = 0,
                dwID = (uint)SIMCONNECT_RECV_ID.EVENT_FILENAME,
                uGroupID = 0,
                uEventID = 10999,
                dwData = 0,
                szFileName = @"SimObjects\Airplanes\Test\aircraft.cfg",
                dwFlags = 0
            };

            var actual = RoundTrip(expected);

            Assert.AreEqual(expected.uEventID, actual.uEventID);
            Assert.AreEqual(expected.szFileName, actual.szFileName);
        }

        [TestMethod()]
        public void Exception_UnmarshalsExceptionCode()
        {
            var expected = new SIMCONNECT_RECV_EXCEPTION
            {
                dwSize = (uint)Marshal.SizeOf<SIMCONNECT_RECV_EXCEPTION>(),
                dwVersion = 0,
                dwID = (uint)SIMCONNECT_RECV_ID.EXCEPTION,
                dwException = (uint)SIMCONNECT_EXCEPTION.ALREADY_CREATED,
                dwSendID = 5,
                dwIndex = 0
            };

            var actual = RoundTrip(expected);

            Assert.AreEqual(SIMCONNECT_EXCEPTION.ALREADY_CREATED, (SIMCONNECT_EXCEPTION)actual.dwException);
        }

        [TestMethod()]
        public void ParseSimObjectData_UnmarshalsRegisteredResponseStringPayload()
        {
            var payload = new ResponseString { Data = "MF.Pong" };
            var typesByDefineId = new Dictionary<uint, Type> { { 5u, typeof(ResponseString) } };

            var result = ParseWithPayload<SIMCONNECT_RECV_CLIENT_DATA, ResponseString>(defineId: 5u, requestId: 99u, payload, typesByDefineId);

            Assert.AreEqual(99u, result.dwRequestID);
            Assert.AreEqual(5u, result.dwDefineID);
            var value = (ResponseString)result.dwData[0];
            Assert.AreEqual("MF.Pong", value.Data);
        }

        [TestMethod()]
        public void ParseSimObjectData_UnmarshalsRegisteredClientDataValuePayload()
        {
            var payload = new ClientDataValue { data = 123.456f };
            var typesByDefineId = new Dictionary<uint, Type> { { 12u, typeof(ClientDataValue) } };

            var result = ParseWithPayload<SIMCONNECT_RECV_CLIENT_DATA, ClientDataValue>(defineId: 12u, requestId: 12u, payload, typesByDefineId);

            var value = (ClientDataValue)result.dwData[0];
            Assert.AreEqual(123.456f, value.data);
        }

        [TestMethod()]
        public void ParseSimObjectData_UnmarshalsRegisteredClientDataStringValuePayload()
        {
            var payload = new ClientDataStringValue { data = "B738" };
            var typesByDefineId = new Dictionary<uint, Type> { { 10001u, typeof(ClientDataStringValue) } };

            var result = ParseWithPayload<SIMCONNECT_RECV_CLIENT_DATA, ClientDataStringValue>(defineId: 10001u, requestId: 10001u, payload, typesByDefineId);

            var value = (ClientDataStringValue)result.dwData[0];
            Assert.AreEqual("B738", value.data);
        }

        [TestMethod()]
        public void ParseSimObjectData_UnregisteredDefineId_ReturnsNullPayloadWithoutThrowing()
        {
            var typesByDefineId = new Dictionary<uint, Type> { { 1u, typeof(ClientDataValue) } };

            var result = ParseWithPayload<SIMCONNECT_RECV_CLIENT_DATA, ClientDataValue>(defineId: 2u, requestId: 2u, new ClientDataValue { data = 1f }, typesByDefineId);

            Assert.AreEqual(2u, result.dwDefineID);
            Assert.IsNull(result.dwData[0]);
        }

        private static T RoundTrip<T>(T value)
        {
            int size = Marshal.SizeOf<T>();
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(value, buffer, false);
                return Marshal.PtrToStructure<T>(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static TRecv ParseWithPayload<TRecv, TPayload>(uint defineId, uint requestId, TPayload payload, Dictionary<uint, Type> typesByDefineId)
            where TRecv : SIMCONNECT_RECV_SIMOBJECT_DATA, new()
        {
            int headerSize = 40; // dwSize,dwVersion,dwID,dwRequestID,dwObjectID,dwDefineID,dwFlags,dwentrynumber,dwoutof,dwDefineCount (10 x uint32)
            int payloadSize = Marshal.SizeOf<TPayload>();
            IntPtr buffer = Marshal.AllocHGlobal(headerSize + payloadSize);
            try
            {
                Marshal.WriteInt32(buffer, 0, headerSize + payloadSize); // dwSize
                Marshal.WriteInt32(buffer, 4, 0);                        // dwVersion
                Marshal.WriteInt32(buffer, 8, (int)SIMCONNECT_RECV_ID.CLIENT_DATA); // dwID
                Marshal.WriteInt32(buffer, 12, (int)requestId);          // dwRequestID
                Marshal.WriteInt32(buffer, 16, 0);                       // dwObjectID
                Marshal.WriteInt32(buffer, 20, (int)defineId);           // dwDefineID
                Marshal.WriteInt32(buffer, 24, 0);                       // dwFlags
                Marshal.WriteInt32(buffer, 28, 0);                       // dwentrynumber
                Marshal.WriteInt32(buffer, 32, 1);                       // dwoutof
                Marshal.WriteInt32(buffer, 36, 1);                       // dwDefineCount

                Marshal.StructureToPtr(payload, IntPtr.Add(buffer, headerSize), false);

                return SimConnect.ParseSimObjectData<TRecv>(buffer, (uint)(headerSize + payloadSize), typesByDefineId);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
