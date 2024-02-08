import { Link, useParams } from 'react-router-dom';
import { Project, Projects } from '../fixtures/projects'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { IconPencil } from '@tabler/icons-react'
import { useTranslation } from 'react-i18next';

export default function ProjectPage() {
    const { t } = useTranslation();
    const params = useParams()
    const project = Projects.find((p: Project) => p.id === params.id)

    return <div className='flex flex-col gap-4'>
        <div className='flex flex-row gap-4 items-center'>
            <Link to="/" className='scroll-m-20 text-3xl tracking-tight first:mt-0'>Projects</Link>
            <p className='scroll-m-20 text-3xl tracking-tight first:mt-0'>&gt;</p>
            <p className='scroll-m-20 text-3xl tracking-tight first:mt-0 font-bold'>{project?.name}</p>
            <IconPencil></IconPencil>
        </div>

        <Card >
            <form action="" className='p-4 flex flex-row gap-4'>
                <div className='w-auto'>Selected simulator</div>
                <div className='font-bold'>{project?.status.sim.name}</div>
                <div className='w-auto'>Linked aircraft</div>
                {project?.linkedAircraft?.map(a => {
                    return <div key={a} className='font-bold'>{a}</div>
                })
                }
            </form>
        </Card>

        <div className='flex flex-row gap-4'>
            <Card className="w-[350px]">
                <CardHeader>
                    <CardTitle>{t("project.configs")}</CardTitle>
                </CardHeader>
                <CardContent>
                    <p>{t("project.status.files")}: {project?.status.configs.files.toString()}</p>
                    <p>{t("project.status.configs")}: {project?.status.configs.configs.toString()}</p>
                    <p>{t("project.status")}: {project?.status.configs.status}</p>
                </CardContent>
            </Card>
            <Card className="w-[350px]">
                <CardHeader>
                    <CardTitle>Devices</CardTitle>
                </CardHeader>
                <CardContent>
                    <p>Number: {project?.status.devices.count.toString()}</p>
                    <p>Status: {project?.status.devices.status}</p>
                </CardContent>
            </Card>
        </div>
    </div>;
}