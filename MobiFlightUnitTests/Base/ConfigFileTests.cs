using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
        public void OpenFileTest()
        {
            String inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            String inFileTemp = @"assets\Base\ConfigFile\temp_OpenFileTest.xml";
            ConfigFile o = new ConfigFile(inFile);
            DataSet InputConfig = new DataSet();
            DataSet OutputConfig = new DataSet();
            initializeOutputDataSet(OutputConfig);
            initializeInputDataSet(InputConfig);
            
            //o.OpenFile();
            OutputConfig.ReadXml(o.getOutputConfig());
            InputConfig.ReadXml(o.getInputConfig());
            
            ConfigFile oTemp = new ConfigFile(inFileTemp);
            oTemp.SaveFile(OutputConfig, InputConfig);

            String s1 = System.IO.File.ReadAllText(inFile);
            String s2 = System.IO.File.ReadAllText(inFileTemp);

            System.IO.File.Delete(inFileTemp);
            Assert.AreEqual(s1, s2, "Files are not the same");

            OutputConfig.Clear();
            InputConfig.Clear();
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
            XmlReader xr = o.getInputConfig();

            Assert.IsNotNull(xr);
        }

        [TestMethod()]
        public void getOutputConfigTest()
        {
            String inFile = @"assets\Base\ConfigFile\OpenFileTest.xml";
            ConfigFile o = new ConfigFile(inFile);
            XmlReader xr = o.getOutputConfig();

            Assert.IsNotNull(xr);
        }

        void initializeOutputDataSet(DataSet dataSetConfig)
        {
            System.Data.DataTable configDataTable;
            System.Data.DataColumn activeDataColumn;
            System.Data.DataColumn fsuipcOffsetDataColumn;
            System.Data.DataColumn converterDataColumn;
            System.Data.DataColumn maskDataColumn;
            System.Data.DataColumn usbArcazePinDataColumn;
            System.Data.DataColumn typeDataColumn;
            System.Data.DataColumn durationDataColumn;
            System.Data.DataColumn comparisonDataColumn;
            System.Data.DataColumn comparisonValueDataColumn;
            System.Data.DataColumn descriptionDataColumn;
            System.Data.DataColumn fsuipcSizeDataColumn;
            System.Data.DataColumn triggerDataColumn;
            System.Data.DataColumn arcazeSerialDataColumn;
            System.Data.DataColumn settingsColumn;
            System.Data.DataColumn guidDataColumn;

            configDataTable = new System.Data.DataTable();
            activeDataColumn = new System.Data.DataColumn();
            fsuipcOffsetDataColumn = new System.Data.DataColumn();
            converterDataColumn = new System.Data.DataColumn();
            maskDataColumn = new System.Data.DataColumn();
            usbArcazePinDataColumn = new System.Data.DataColumn();
            typeDataColumn = new System.Data.DataColumn();
            durationDataColumn = new System.Data.DataColumn();
            comparisonDataColumn = new System.Data.DataColumn();
            comparisonValueDataColumn = new System.Data.DataColumn();
            descriptionDataColumn = new System.Data.DataColumn();
            fsuipcSizeDataColumn = new System.Data.DataColumn();
            triggerDataColumn = new System.Data.DataColumn();
            arcazeSerialDataColumn = new System.Data.DataColumn();
            settingsColumn = new System.Data.DataColumn();
            guidDataColumn = new System.Data.DataColumn();
            // 
            // dataSetConfig
            // 
            dataSetConfig.DataSetName = "outputs";
            dataSetConfig.Tables.AddRange(new System.Data.DataTable[] {
            configDataTable});
            // 
            // configDataTable
            // 
            configDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            activeDataColumn,
            fsuipcOffsetDataColumn,
            converterDataColumn,
            maskDataColumn,
            usbArcazePinDataColumn,
            typeDataColumn,
            durationDataColumn,
            comparisonDataColumn,
            comparisonValueDataColumn,
            descriptionDataColumn,
            fsuipcSizeDataColumn,
            triggerDataColumn,
            arcazeSerialDataColumn,
            settingsColumn,
            guidDataColumn});
            configDataTable.TableName = "config";
            // 
            // activeDataColumn
            // 
            activeDataColumn.Caption = "Active";
            activeDataColumn.ColumnName = "active";
            activeDataColumn.DataType = typeof(bool);
            activeDataColumn.DefaultValue = false;
            // 
            // fsuipcOffsetDataColumn
            // 
            fsuipcOffsetDataColumn.Caption = "FsuipcOffset";
            fsuipcOffsetDataColumn.ColumnName = "fsuipcOffset";
            // 
            // converterDataColumn
            // 
            converterDataColumn.Caption = "Converter";
            converterDataColumn.ColumnName = "converter";
            converterDataColumn.DefaultValue = "Boolean";
            // 
            // maskDataColumn
            // 
            maskDataColumn.Caption = "Mask";
            maskDataColumn.ColumnName = "mask";
            // 
            // usbArcazePinDataColumn
            // 
            usbArcazePinDataColumn.Caption = "USBArcazePin";
            usbArcazePinDataColumn.ColumnName = "usbArcazePin";
            // 
            // typeDataColumn
            // 
            typeDataColumn.Caption = "Type";
            typeDataColumn.ColumnName = "type";
            // 
            // durationDataColumn
            // 
            durationDataColumn.Caption = "Duration";
            durationDataColumn.ColumnName = "duration";
            // 
            // comparisonDataColumn
            // 
            comparisonDataColumn.Caption = "Comparison";
            comparisonDataColumn.ColumnName = "comparison";
            comparisonDataColumn.DefaultValue = "=";
            // 
            // comparisonValueDataColumn
            // 
            comparisonValueDataColumn.Caption = "ComparisonValue";
            comparisonValueDataColumn.ColumnName = "comparisonValue";
            // 
            // descriptionDataColumn
            // 
            descriptionDataColumn.AllowDBNull = false;
            descriptionDataColumn.Caption = "Description";
            descriptionDataColumn.ColumnName = "description";
            descriptionDataColumn.DefaultValue = "";
            // 
            // fsuipcSizeDataColumn
            // 
            fsuipcSizeDataColumn.Caption = "Fsuipc Size";
            fsuipcSizeDataColumn.ColumnName = "fsuipcSize";
            fsuipcSizeDataColumn.DefaultValue = "1";
            fsuipcSizeDataColumn.MaxLength = 3;
            // 
            // triggerDataColumn
            // 
            triggerDataColumn.ColumnName = "trigger";
            triggerDataColumn.DefaultValue = "change";
            // 
            // arcazeSerialDataColumn
            // 
            arcazeSerialDataColumn.ColumnName = "arcazeSerial";
            arcazeSerialDataColumn.DefaultValue = "";
            // 
            // settingsColumn
            // 
            settingsColumn.Caption = "settings";
            settingsColumn.ColumnName = "settings";
            settingsColumn.DataType = typeof(object);
            // 
            // guidDataColumn
            // 
            guidDataColumn.ColumnMapping = System.Data.MappingType.Attribute;
            guidDataColumn.ColumnName = "guid";
            guidDataColumn.DataType = typeof(System.Guid);
        }

        void initializeInputDataSet (DataSet dataSetInputs)
        {
            System.Data.DataTable inputsDataTable;
            System.Data.DataColumn inputsActiveDataColumn;
            System.Data.DataColumn inputsDescriptionDataColumn;
            System.Data.DataColumn inputsSettingsDataColumn;
            System.Data.DataColumn inputsGuidDataColumn;
            
            inputsDataTable = new System.Data.DataTable();
            inputsActiveDataColumn = new System.Data.DataColumn();
            inputsDescriptionDataColumn = new System.Data.DataColumn();
            inputsGuidDataColumn = new System.Data.DataColumn();
            inputsSettingsDataColumn = new System.Data.DataColumn();

            // 
            // dataSetInputs
            // 
            dataSetInputs.DataSetName = "inputs";
            dataSetInputs.Tables.AddRange(new System.Data.DataTable[] {
            inputsDataTable});
            // 
            // inputsDataTable
            // 
            inputsDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            inputsActiveDataColumn,
            inputsDescriptionDataColumn,
            inputsGuidDataColumn,
            inputsSettingsDataColumn});
            inputsDataTable.TableName = "config";
            
            // 
            // inputsActiveDataColumn
            // 
            inputsActiveDataColumn.Caption = "Active";
            inputsActiveDataColumn.ColumnName = "active";
            inputsActiveDataColumn.DataType = typeof(bool);
            inputsActiveDataColumn.DefaultValue = false;
            // 
            // inputsDescriptionDataColumn
            // 
            inputsDescriptionDataColumn.AllowDBNull = false;
            inputsDescriptionDataColumn.Caption = "Description";
            inputsDescriptionDataColumn.ColumnName = "description";
            inputsDescriptionDataColumn.DefaultValue = "";
            // 
            // inputsGuidDataColumn
            // 
            inputsGuidDataColumn.ColumnMapping = System.Data.MappingType.Attribute;
            inputsGuidDataColumn.ColumnName = "guid";
            inputsGuidDataColumn.DataType = typeof(System.Guid);
            // 
            // inputsSettingsDataColumn
            // 
            inputsSettingsDataColumn.ColumnName = "settings";
            inputsSettingsDataColumn.DataType = typeof(object);
        }
    }
}