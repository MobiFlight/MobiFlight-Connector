import { Label } from '@radix-ui/react-label'
import { Select, SelectTrigger, SelectValue, SelectContent, SelectItem } from '@radix-ui/react-select'
import React from 'react'
import { Button } from '../ui/button'
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from '../ui/card'
import { Input } from '../ui/input'
import { Project } from '@/fixtures/projects'
import { Badge } from "@/components/ui/badge"
import { Link } from 'react-router-dom'

type ProjectCardProps = {
    project: Project
}

const ProjectCard = (props: ProjectCardProps) => {
    const project = props.project
    return (
        <Card className="w-[350px]">
            <CardHeader>
                <CardTitle>{project.name}</CardTitle>
                <CardDescription><Badge className='bg-green-700' variant="default">{project.status.sim.name}</Badge></CardDescription>
                {
                    //<CardDescription>Deploy your new project in one-click.</CardDescription>
                }
            </CardHeader>
            <CardContent>
                <div className="grid w-full items-center gap-4">
                    <div className="flex flex-col justify-between gap-0">
                        <p className='font-bold'>Aircraft</p>
                        <p>{ project.linkedAircraft?.join(', ') }</p>
                    </div>
                    <div className="flex flex-row justify-between content-baseline text-center">
                        <div>
                            <p>Devices</p>
                            <Badge color="" variant="outline">1 of {project.status.devices.count.toString()}</Badge>
                        </div>
                        <div>
                            <p>Configs</p>
                            <Badge variant="outline">{project.status.configs.configs.toString()}</Badge>
                        </div>
                        <div>
                            <p>Files</p>
                            <Badge variant="outline">{project.status.configs.files.toString()}</Badge>
                        </div>
                    </div>
                </div>
            </CardContent>
            <CardFooter className="flex justify-end">
                <Link to={`/projects/${project.id}`}><Button>Open</Button></Link>
            </CardFooter>
        </Card>
    )
}

export default ProjectCard