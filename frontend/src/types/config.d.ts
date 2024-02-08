export default interface IConfigItem {
    Active: boolean;
    Description: string;
    Device: string;
    Component: string;
    Type: string;
    Tags: string[];
    Status: string[];
    RawValue: string;
    ModifiedValue: string;
}