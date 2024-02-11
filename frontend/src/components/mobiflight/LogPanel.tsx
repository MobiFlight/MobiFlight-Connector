import { columns } from '@/pages/config/log-columns'
import { LogDataTable } from '@/pages/config/log-data-table'
import { useLogMessageStore } from '@/stores/logStore'

const LogPanel = () => {
    const { messages } = useLogMessageStore()

    return (
        <LogDataTable columns={columns} data={messages} />
    )
}

export default LogPanel