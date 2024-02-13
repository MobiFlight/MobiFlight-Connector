import { Project, Projects } from './../fixtures/projects'
import { useConfigStore } from '@/stores/configFileStore';
import { Link, useParams } from 'react-router-dom';
import { IconPencil } from '@tabler/icons-react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { DataTable } from './config/data-table';
import { columns } from './config/config-columns';

const ConfigPage = () => {    
    const params = useParams()
    const { items: configItems } = useConfigStore()
    const project = Projects.find((p: Project) => p.id === params.id)

    return (
        <div className='h-full flex flex-col gap-4 overflow-y-auto'>
            <div className='flex flex-row gap-4 items-center'>
                <Link to="/" className='scroll-m-20 text-3xl tracking-tight first:mt-0'>Projects</Link>
                <p className='scroll-m-20 text-3xl tracking-tight first:mt-0'>&gt;</p>
                <p className='scroll-m-20 text-3xl tracking-tight first:mt-0'>{project?.name}</p>
                <p className='scroll-m-20 text-3xl tracking-tight first:mt-0'>&gt;</p>
                <p className='scroll-m-20 text-3xl tracking-tight first:mt-0 font-bold'>Configs</p>
                <IconPencil></IconPencil>
            </div>

            <Tabs defaultValue="config-1" className='grow flex flex-col overflow-y-auto'>
                <div>
                    <TabsList className='mb-4 mt-0'>
                        <TabsTrigger value="config-1">Config one</TabsTrigger>
                        <TabsTrigger value="config-2">Autopilot</TabsTrigger>
                        <TabsTrigger value="config-3">Radio</TabsTrigger>
                    </TabsList>
                </div>
                <TabsContent value="config-1" className='mt-0 flex flex-col grow overflow-y-auto'>                   
                    <DataTable columns={columns} data={configItems} />
                </TabsContent>
            </Tabs>
        </div>
    )
}

export default ConfigPage