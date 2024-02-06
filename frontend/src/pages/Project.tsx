import { useParams } from 'react-router-dom';
import { Project, Projects } from '../fixtures/projects'

export default function ProjectPage() {
    const params = useParams()
    const project = Projects.find((p : Project) => p.id === params.id)
    return <>
        <h1>Project</h1>
        <p>Breadcrumb: Projects &gt; {project?.name}</p>
        <h2>Configs</h2>
        <p>Files: {project?.status.configs.files.toString()}</p>
        <p>Configs: {project?.status.configs.configs.toString()}</p>
        <p>Status: {project?.status.configs.status}</p>
        <h2>Devices</h2>
        <p>Number: {project?.status.devices.count.toString()}</p>
        <p>Status: {project?.status.devices.status}</p>
    </>;
}