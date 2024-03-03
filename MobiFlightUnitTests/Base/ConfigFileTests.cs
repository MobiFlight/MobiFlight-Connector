using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class ConfigFileTests
    {
        [TestMethod()]
        public void ConfigFileTest()
        {

            ConfigFile o = new ConfigFile(@"assets\Base\ConfigFile\OpenConfigFile.xml");
            Assert.IsNotNull(o, "Object is null");
        }

        [TestMethod()]
        //[Ignore]
        public void OpenFileTest()
        {
            String inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            String expFile = @"assets\Base\ConfigFile\OpenFileTest.xml.exp";
            String inFileTemp = @"assets\Base\ConfigFile\temp_OpenFileTest.xml";

            ConfigFile o = new ConfigFile(inFile);
            o.OpenFile();

            ConfigFile oTemp = new ConfigFile(inFileTemp);
            oTemp.OutputConfigItems = o.GetOutputConfigItems();
            oTemp.InputConfigItems = o.GetInputConfigItems();
            oTemp.SaveFile();

            String s1 = System.IO.File.ReadAllText(expFile);
            String s2 = System.IO.File.ReadAllText(inFileTemp);

            // get rid of the individual version number
            // that varies every time we increment the MobiFlight version
            s1 = replaceTestIrrelevantXmlInformation(s1);
            s2 = replaceTestIrrelevantXmlInformation(s2);

            Assert.AreEqual(s1, s2, inFile + ": Files are not the same");
            System.IO.File.Delete(inFileTemp);


            inFile = @"assets\Base\ConfigFile\OpenFileTest.2912.xml";
            expFile = @"assets\Base\ConfigFile\OpenFileTest.2912.xml.exp";
            inFileTemp = @"assets\Base\ConfigFile\temp_OpenFileTest.2912.xml";

            o = new ConfigFile(inFile);
            o.OpenFile();

            oTemp = new ConfigFile(inFileTemp);
            oTemp.OutputConfigItems = o.GetOutputConfigItems();
            oTemp.InputConfigItems = o.GetInputConfigItems();
            oTemp.SaveFile();

            s1 = System.IO.File.ReadAllText(expFile);
            s2 = System.IO.File.ReadAllText(inFileTemp);

            // get rid of the individual version number
            // that varies every time we increment the MobiFlight version
            s1 = replaceTestIrrelevantXmlInformation(s1);
            s2 = replaceTestIrrelevantXmlInformation(s2);

            Assert.AreEqual(s1, s2, inFile + ": Files are not the same");
            System.IO.File.Delete(inFileTemp);

            inFile = @"assets\Base\ConfigFile\7.5.0-7.5.1-upgrade.mcc";
            inFileTemp = @"assets\Base\ConfigFile\7.5.0-7.5.1-upgrade.mcc.tmp";
            expFile = @"assets\Base\ConfigFile\7.5.0-7.5.1-upgrade.mcc.exp";

            o = new ConfigFile(inFile);
            o.OpenFile();

            oTemp = new ConfigFile(inFileTemp);
            oTemp.OutputConfigItems = o.GetOutputConfigItems();
            oTemp.InputConfigItems = o.GetInputConfigItems();
            oTemp.SaveFile();

            s1 = System.IO.File.ReadAllText(expFile);
            s2 = System.IO.File.ReadAllText(inFileTemp);

            // get rid of the individual version number
            // that varies every time we increment the MobiFlight version
            s1 = replaceTestIrrelevantXmlInformation(s1);
            s2 = replaceTestIrrelevantXmlInformation(s2);

            Assert.AreEqual(s1, s2, inFile + ": Files are not the same");
            System.IO.File.Delete(inFileTemp);

            // Load the new version was problematic\
            inFile = @"assets\Base\ConfigFile\7.5.0-7.5.1-upgrade.mcc.exp";
            inFileTemp = @"assets\Base\ConfigFile\7.5.0-7.5.1-upgrade.mcc.tmp";
            expFile = @"assets\Base\ConfigFile\7.5.0-7.5.1-upgrade.mcc.exp";

            o = new ConfigFile(inFile);
            o.OpenFile();

            oTemp = new ConfigFile(inFileTemp);
            oTemp.OutputConfigItems = o.GetOutputConfigItems();
            oTemp.InputConfigItems = o.GetInputConfigItems();
            oTemp.SaveFile();

            s1 = System.IO.File.ReadAllText(expFile);
            s2 = System.IO.File.ReadAllText(inFileTemp);

            // get rid of the individual version number
            // that varies every time we increment the MobiFlight version
            s1 = replaceTestIrrelevantXmlInformation(s1);
            s2 = replaceTestIrrelevantXmlInformation(s2);

            Assert.AreEqual(s1, s2, inFile + ": Files are not the same");
            System.IO.File.Delete(inFileTemp);

            // the files are partially from this project
            // and also the example files are copied over
            // from the examples folder (Main project)
            foreach (string file in System.IO.Directory.GetFiles(@"assets\Base\ConfigFile\", "*.mcc"))
            {
                inFile = file;
                expFile = inFile;
                if (System.IO.File.Exists(inFile + ".exp")) expFile = inFile + ".exp";
                inFileTemp = inFile + ".tmp";

                o = new ConfigFile(inFile);
                o.OpenFile();

                oTemp = new ConfigFile(inFileTemp);
                oTemp.OutputConfigItems = o.GetOutputConfigItems();
                oTemp.InputConfigItems = o.GetInputConfigItems();
                oTemp.SaveFile();

                s1 = System.IO.File.ReadAllText(expFile);
                s2 = System.IO.File.ReadAllText(inFileTemp);

                // get rid of the individual version number
                // that varies every time we increment the MobiFlight version
                s1 = replaceTestIrrelevantXmlInformation(s1);
                s2 = replaceTestIrrelevantXmlInformation(s2);

                Assert.AreEqual(s1, s2, inFile + ": Files are not the same");
                System.IO.File.Delete(inFileTemp);
            }

            // load a broken config
            inFile = @"assets\Base\ConfigFile\OpenFileBrokenTest.xml";
            inFileTemp = @"assets\Base\ConfigFile\OpenFileBrokenTest.xml.tmp";

            bool exceptionThrown = false;

            try
            {
                s1 = "a";
                s2 = "b";
                o = new ConfigFile(inFile);
                o.OpenFile();

                oTemp = new ConfigFile(inFileTemp);
                oTemp.OutputConfigItems = o.GetOutputConfigItems();
                oTemp.InputConfigItems = o.GetInputConfigItems();
                oTemp.SaveFile();

                s1 = System.IO.File.ReadAllText(inFile);
                s2 = System.IO.File.ReadAllText(inFileTemp);

                // get rid of the individual version number
                // that varies every time we increment the MobiFlight version
                s1 = replaceTestIrrelevantXmlInformation(s1);
                s2 = replaceTestIrrelevantXmlInformation(s2);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod()]
        public void SaveFileTest()
        {
            // implicitly tested in OpenFileTest();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void getInputConfigTest()
        {
            String inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            ConfigFile o = new ConfigFile(inFile);
            o.OpenFile();
            var xr = o.GetInputConfigItems();

            Assert.IsNotNull(xr);
        }

        [TestMethod()]
        public void getOutputConfigTest()
        {
            String inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            ConfigFile o = new ConfigFile(inFile);
            o.OpenFile();
            var xr = o.GetOutputConfigItems();

            Assert.IsNotNull(xr);
        }

        string replaceTestIrrelevantXmlInformation(string s1)
        {
            s1 = Regex.Replace(s1, @"Version=[^,]+", "Version=1.0.0.0");
            s1 = s1.Replace(" msdata:InstanceType=\"MobiFlight.OutputConfigItem, MFConnector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"", "");
            s1 = s1.Replace(" msdata:InstanceType=\"MobiFlight.InputConfigItem, MFConnector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"", "");
            s1 = s1.Replace(" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\"", "");
            s1 = s1.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            return s1;
        }

        //void initializeOutputDataSet(DataSet dataSetConfig)
        //{
        //    System.Data.DataTable configDataTable;
        //    System.Data.DataColumn activeDataColumn;
        //    System.Data.DataColumn fsuipcOffsetDataColumn;
        //    System.Data.DataColumn converterDataColumn;
        //    System.Data.DataColumn maskDataColumn;
        //    System.Data.DataColumn usbArcazePinDataColumn;
        //    System.Data.DataColumn typeDataColumn;
        //    System.Data.DataColumn durationDataColumn;
        //    System.Data.DataColumn comparisonDataColumn;
        //    System.Data.DataColumn comparisonValueDataColumn;
        //    System.Data.DataColumn descriptionDataColumn;
        //    System.Data.DataColumn fsuipcSizeDataColumn;
        //    System.Data.DataColumn triggerDataColumn;
        //    System.Data.DataColumn arcazeSerialDataColumn;
        //    System.Data.DataColumn settingsColumn;
        //    System.Data.DataColumn guidDataColumn;

        //    configDataTable = new System.Data.DataTable();
        //    activeDataColumn = new System.Data.DataColumn();
        //    fsuipcOffsetDataColumn = new System.Data.DataColumn();
        //    converterDataColumn = new System.Data.DataColumn();
        //    maskDataColumn = new System.Data.DataColumn();
        //    usbArcazePinDataColumn = new System.Data.DataColumn();
        //    typeDataColumn = new System.Data.DataColumn();
        //    durationDataColumn = new System.Data.DataColumn();
        //    comparisonDataColumn = new System.Data.DataColumn();
        //    comparisonValueDataColumn = new System.Data.DataColumn();
        //    descriptionDataColumn = new System.Data.DataColumn();
        //    fsuipcSizeDataColumn = new System.Data.DataColumn();
        //    triggerDataColumn = new System.Data.DataColumn();
        //    arcazeSerialDataColumn = new System.Data.DataColumn();
        //    settingsColumn = new System.Data.DataColumn();
        //    guidDataColumn = new System.Data.DataColumn();
        //    // 
        //    // dataSetConfig
        //    // 
        //    dataSetConfig.DataSetName = "outputs";
        //    dataSetConfig.Tables.AddRange(new System.Data.DataTable[] {
        //    configDataTable});
        //    // 
        //    // configDataTable
        //    // 
        //    configDataTable.Columns.AddRange(new System.Data.DataColumn[] {
        //    activeDataColumn,
        //    fsuipcOffsetDataColumn,
        //    converterDataColumn,
        //    maskDataColumn,
        //    usbArcazePinDataColumn,
        //    typeDataColumn,
        //    durationDataColumn,
        //    comparisonDataColumn,
        //    comparisonValueDataColumn,
        //    descriptionDataColumn,
        //    fsuipcSizeDataColumn,
        //    triggerDataColumn,
        //    arcazeSerialDataColumn,
        //    settingsColumn,
        //    guidDataColumn});
        //    configDataTable.TableName = "config";
        //    // 
        //    // activeDataColumn
        //    // 
        //    activeDataColumn.Caption = "Active";
        //    activeDataColumn.ColumnName = "active";
        //    activeDataColumn.DataType = typeof(bool);
        //    activeDataColumn.DefaultValue = false;
        //    // 
        //    // fsuipcOffsetDataColumn
        //    // 
        //    fsuipcOffsetDataColumn.Caption = "FsuipcOffset";
        //    fsuipcOffsetDataColumn.ColumnName = "fsuipcOffset";
        //    // 
        //    // converterDataColumn
        //    // 
        //    converterDataColumn.Caption = "Converter";
        //    converterDataColumn.ColumnName = "converter";
        //    converterDataColumn.DefaultValue = "Boolean";
        //    // 
        //    // maskDataColumn
        //    // 
        //    maskDataColumn.Caption = "Mask";
        //    maskDataColumn.ColumnName = "mask";
        //    // 
        //    // usbArcazePinDataColumn
        //    // 
        //    usbArcazePinDataColumn.Caption = "USBArcazePin";
        //    usbArcazePinDataColumn.ColumnName = "usbArcazePin";
        //    // 
        //    // typeDataColumn
        //    // 
        //    typeDataColumn.Caption = "Type";
        //    typeDataColumn.ColumnName = "type";
        //    // 
        //    // durationDataColumn
        //    // 
        //    durationDataColumn.Caption = "Duration";
        //    durationDataColumn.ColumnName = "duration";
        //    // 
        //    // comparisonDataColumn
        //    // 
        //    comparisonDataColumn.Caption = "Comparison";
        //    comparisonDataColumn.ColumnName = "comparison";
        //    comparisonDataColumn.DefaultValue = "=";
        //    // 
        //    // comparisonValueDataColumn
        //    // 
        //    comparisonValueDataColumn.Caption = "ComparisonValue";
        //    comparisonValueDataColumn.ColumnName = "comparisonValue";
        //    // 
        //    // descriptionDataColumn
        //    // 
        //    descriptionDataColumn.AllowDBNull = false;
        //    descriptionDataColumn.Caption = "Description";
        //    descriptionDataColumn.ColumnName = "description";
        //    descriptionDataColumn.DefaultValue = "";
        //    // 
        //    // fsuipcSizeDataColumn
        //    // 
        //    fsuipcSizeDataColumn.Caption = "Fsuipc Size";
        //    fsuipcSizeDataColumn.ColumnName = "fsuipcSize";
        //    fsuipcSizeDataColumn.DefaultValue = "1";
        //    fsuipcSizeDataColumn.MaxLength = 3;
        //    // 
        //    // triggerDataColumn
        //    // 
        //    triggerDataColumn.ColumnName = "trigger";
        //    triggerDataColumn.DefaultValue = "change";
        //    // 
        //    // arcazeSerialDataColumn
        //    // 
        //    arcazeSerialDataColumn.ColumnName = "arcazeSerial";
        //    arcazeSerialDataColumn.DefaultValue = "";
        //    // 
        //    // settingsColumn
        //    // 
        //    settingsColumn.Caption = "settings";
        //    settingsColumn.ColumnName = "settings";
        //    settingsColumn.DataType = typeof(object);
        //    // 
        //    // guidDataColumn
        //    // 
        //    guidDataColumn.ColumnMapping = System.Data.MappingType.Attribute;
        //    guidDataColumn.ColumnName = "guid";
        //    guidDataColumn.DataType = typeof(System.Guid);
        //}

        //void initializeInputDataSet (DataSet dataSetInputs)
        //{
        //    System.Data.DataTable inputsDataTable;
        //    System.Data.DataColumn inputsActiveDataColumn;
        //    System.Data.DataColumn inputsDescriptionDataColumn;
        //    System.Data.DataColumn inputsSettingsDataColumn;
        //    System.Data.DataColumn inputsGuidDataColumn;

        //    inputsDataTable = new System.Data.DataTable();
        //    inputsActiveDataColumn = new System.Data.DataColumn();
        //    inputsDescriptionDataColumn = new System.Data.DataColumn();
        //    inputsGuidDataColumn = new System.Data.DataColumn();
        //    inputsSettingsDataColumn = new System.Data.DataColumn();

        //    // 
        //    // dataSetInputs
        //    // 
        //    dataSetInputs.DataSetName = "inputs";
        //    dataSetInputs.Tables.AddRange(new System.Data.DataTable[] {
        //    inputsDataTable});
        //    // 
        //    // inputsDataTable
        //    // 
        //    inputsDataTable.Columns.AddRange(new System.Data.DataColumn[] {
        //    inputsActiveDataColumn,
        //    inputsDescriptionDataColumn,
        //    inputsGuidDataColumn,
        //    inputsSettingsDataColumn});
        //    inputsDataTable.TableName = "config";

        //    // 
        //    // inputsActiveDataColumn
        //    // 
        //    inputsActiveDataColumn.Caption = "Active";
        //    inputsActiveDataColumn.ColumnName = "active";
        //    inputsActiveDataColumn.DataType = typeof(bool);
        //    inputsActiveDataColumn.DefaultValue = false;
        //    // 
        //    // inputsDescriptionDataColumn
        //    // 
        //    inputsDescriptionDataColumn.AllowDBNull = false;
        //    inputsDescriptionDataColumn.Caption = "Description";
        //    inputsDescriptionDataColumn.ColumnName = "description";
        //    inputsDescriptionDataColumn.DefaultValue = "";
        //    // 
        //    // inputsGuidDataColumn
        //    // 
        //    inputsGuidDataColumn.ColumnMapping = System.Data.MappingType.Attribute;
        //    inputsGuidDataColumn.ColumnName = "guid";
        //    inputsGuidDataColumn.DataType = typeof(System.Guid);
        //    // 
        //    // inputsSettingsDataColumn
        //    // 
        //    inputsSettingsDataColumn.ColumnName = "settings";
        //    inputsSettingsDataColumn.DataType = typeof(object);
        //}
    }
}