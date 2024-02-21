import { columns } from '@/components/mobiflight/tables/log-columns'
import { LogDataTable } from '@/components/mobiflight/tables/log-data-table'
import { useLogMessageStore } from '@/stores/logStore'

const LogPanel = () => {
    const { messages } = useLogMessageStore()
    return (
        <LogDataTable columns={columns} data={messages} />
    )
}

export default LogPanel