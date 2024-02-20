export interface IConfigItem {
    GUID: string
    Active: boolean
    Description: string
    Device: string
    Component: string
    Type: string
    Tags: string[]
    Status: string[]
    RawValue: string
    ModifiedValue: string
}

interface IDictionary<T> {
    [Key: string]: T
}

export interface IDeviceItem {
    Id: string
    Type: string
    Name: string
    MetaData: IDictionary<string>
}