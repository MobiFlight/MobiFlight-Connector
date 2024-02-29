import { Projects } from '../../tests/fixtures/data/projects'
import ProjectCard from '@/components/mobiflight/ProjectCard';

const StartPage = () => {
    const projects = Projects
    return (
        <>
            <h2 className="scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight first:mt-0">Projects</h2>
            <div className='flex flex-row pt-4 w-full gap-4 flex-wrap max-w-full h-full'>
                {
                    projects.map(p => {
                        return <ProjectCard key={p.id} project={p} />
                    })

                }
            </div>
        </>
    )
}

export default StartPage