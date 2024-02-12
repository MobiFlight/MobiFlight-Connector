import LogPanel from '@/components/mobiflight/LogPanel'

const LogPage = () => {
    
  return (
    <div className='h-full flex flex-col gap-4 overflow-y-auto'>
        <h2 className="pb-4 text-3xl font-semibold tracking-tight first:mt-0">Logs</h2>
        <div className='grow flex flex-col overflow-y-auto'>
            <LogPanel />
        </div>
    </div>
  )
}


export default LogPage