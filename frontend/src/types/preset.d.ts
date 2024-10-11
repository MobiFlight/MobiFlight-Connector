export interface Preset {
    id?: string;
    path: string;
    vendor: string;
    aircraft: string;
    system: string;
    code: string;
    label: string;
    presetType: string;
    status : string;
    createdDate : Date;
    author : string;
    version : string;
    description : string;
    updatedBy : string;
    score : number;
    reported : number;
    report_catergory : string;
    report_description : string;
    votes : number;
    codeType : string;
}