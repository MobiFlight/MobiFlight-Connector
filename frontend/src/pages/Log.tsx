import LogPanel from '@/components/mobiflight/LogPanel'

const LogPage = () => {
    
  return (
    <>
        <h2 className="scroll-m-20 pb-4 text-3xl font-semibold tracking-tight first:mt-0">Logs</h2>
        <div className='md:w-full 2xl:max-fhd:w-5/6 fhd:w-5/6 grow flex flex-col'>
            <LogPanel />
        </div>
    </>
  )
}


export default LogPage