// Mirrors the C# enum FSUIPCOffsetType (MobiFlight.FSUIPC.FSUIPCOffsetType),
// which is used by MobiFlight.ConnectorValue.type and serialized as a number.
// Keep the numeric values in sync with the C# declaration order.
export const ConnectorValueType = {
  Integer: 0,
  Float: 1,
  String: 2,
} as const

export type ConnectorValueType =
  (typeof ConnectorValueType)[keyof typeof ConnectorValueType]
