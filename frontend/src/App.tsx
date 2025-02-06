import { Outlet, useNavigate, useOutlet, useSearchParams } from 'react-router'
import StartupProgress from './components/StartupProgress'
import { useEffect, useState } from 'react'
import { useAppMessage } from './lib/hooks/appMessage'
import { ConfigLoadedEvent, StatusBarUpdate } from './types'
import { useConfigStore } from './stores/configFileStore'

function App() {  
  const [queryParameters] = useSearchParams()
  const navigate = useNavigate()
  const { setItems } = useConfigStore();

  const [startupProgress, setStartupProgress] = useState<StatusBarUpdate>(
    { Value: 0, Text: "Starting..." },
  )

  useAppMessage("StatusBarUpdate", (message) => {
    setStartupProgress(message.payload as StatusBarUpdate)
  })

  useAppMessage("ConfigFile", (message) => {
    console.log("ConfigFile message received", message.payload)
    setItems((message.payload as ConfigLoadedEvent).ConfigItems)
  })

  // this allows to get beyond the startup screen
  // by setting the progress to 100 via url parameter
  useEffect(() => {
    // convert string to number    
    const value = Number.parseInt(queryParameters.get("progress")?.toString() ?? "0")
    if (value == 100) {
      console.log("Finished loading, navigating to config page")
      navigate("/config")
    }
    else
      setStartupProgress({ Value: value, Text: "Loading..." })
  }, [navigate, queryParameters])

  useEffect(() => {
    if (startupProgress.Value == 100) {
      console.log("Finished loading, navigating to config page")
      navigate("/config")
    }
  }, [startupProgress.Value, navigate])

  const outlet = useOutlet()

  return (
    <>
    { outlet ? (
      <div className="flex h-svh flex-row p-2">
        {/* <Sidebar /> */}
        <div className="flex grow flex-col">
          {/* <Navbar /> */}
          <div className="flex grow flex-col overflow-hidden">
            <Outlet />
          </div>
          <div className="flex flex-row justify-end gap-2 bg-white px-5 dark:bg-zinc-800">
            <div className="text-xs text-gray-500">MobiFlight 2025</div>
            <div className="text-xs text-gray-500">Version 1.0.0</div>
          </div>
        </div>
        {/* <Toaster
          position="bottom-right"
          offset={48}
        /> */}
      </div>
    ) : (
    <StartupProgress
        value={startupProgress.Value}
        text={startupProgress.Text}
      />)
    }
    </>
  )
}

export default App
