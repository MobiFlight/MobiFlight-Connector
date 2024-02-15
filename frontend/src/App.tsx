import './App.css'
import { Outlet, useNavigate, useSearchParams } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Navbar from './components/Navbar';
import logo from './assets/mobiflight-logo-border.png'

// import i18n (needs to be bundled ;)) 
import './i18n';
import { useEffect, useState } from 'react';
import { Progress } from './components/ui/progress';
import * as Types from './types/index';
import { useConfigStore } from './stores/configFileStore';
import { useGlobalSettingsStore } from './stores/globalSettingsStore';
import { useLogMessageStore } from './stores/logStore';
import { useExecutionStateStore } from './stores/executionStateStore';

function App() {
  const navigate = useNavigate();
  const { setItems, updateItem } = useConfigStore()
  const { setSettings } = useGlobalSettingsStore()
  const { addMessage } = useLogMessageStore()
  const { setState } = useExecutionStateStore()

  const handleMessage = (message: any) => {
    var eventData = JSON.parse(message.data) as Types.AppMessage
    console.log(`Handle AppMessage -> ${eventData.key}`)

    if (eventData.key == 'config.update') {
      const updatedItem = eventData.payload as Types.IConfigItem
      const items = useConfigStore.getState().items

      console.log(updatedItem)
      console.log(items)
      updateItem(updatedItem)

      const newList = items.map((item) => {
        return item.GUID === updatedItem.GUID ? updatedItem : item
      })

      console.log(newList)
      return
    }

    if (eventData.key == "GlobalSettings") {
      const settings = eventData.payload as Types.IGlobalSettings
      console.log("Global Settings Loaded")
      setSettings(settings)
    }

    if (eventData.key == "StatusBarUpdate") {
      const update = eventData.payload as Types.StatusBarUpdate
      setStartupProgress(update)
    }

    if (eventData.key === "ConfigFile") {
      setStartupProgress({ Value: 100, Text: "Finished!" })
      console.log(eventData.payload)
      const configFile = eventData.payload as Types.ConfigLoadedEvent
      console.log("Config File Loaded")
      setItems(configFile.ConfigItems)
      navigate(`/projects/1`);
    }

    if (eventData.key === "LogMessage") {
      const logMessage = eventData.payload as Types.ILogMessage
      addMessage(logMessage)
    }

    if (eventData.key === "ExecutionUpdate") {
      const update = eventData.payload as Types.ExecutionUpdate
      setState(update.State)
    }

    if (eventData.key === "ConfigValueUpdate") {
      //console.log("Config Value Update")
      const update = eventData.payload as Types.ConfigValueUpdate
      console.log(update)
      // i want to update the items in state
      // and replace them with the new ones
      // using GUID as the unique identifier
      const mergedItems = useConfigStore.getState().items.map((item) => {
        const newItem = update.ConfigItems.find((newItem) => newItem.GUID === item.GUID)
        return newItem ? newItem : item
      })
      setItems(mergedItems)
    }
  }

  const [queryParameters] = useSearchParams()
  const [startupProgress, setStartupProgress] = useState<Types.StatusBarUpdate>({ Value: 0, Text: "Starting..." })

  useEffect(() => {
    window.chrome?.webview?.addEventListener('message', handleMessage)
    return () => {
      window.chrome?.webview?.removeEventListener('message', handleMessage)
      window.removeEventListener('beforeunload', handleMessage);
    };
  }, [])

  useEffect(() => {
    const handleBeforeUnload = (event: any) => {
      if ((event.which || event.keyCode) == 116) event.preventDefault();
    };
    window.addEventListener('keydown', handleBeforeUnload);
    return () => {
      window.removeEventListener('keydown', handleBeforeUnload);
    };
  }, []);

  useEffect(() => {
    if (queryParameters.get("progress") == "100") setStartupProgress({ Value: 100, Text: "Finished!" })
  }, [])

  return (
    <>
      {
        startupProgress.Value < 100
          ?
          <>
            <div className="fixed inset-0 bg-[url('/assets/background-nologo.jpg')]"></div>
            <div className="fixed inset-0 bg-gradient-to-br from-indigo-500 from-10% via-sky-500 via-30% to-emerald-500 to-90%"></div>
            <div className='relative flex flex-col justify-center min-h-screen p-10 items-center gap-8'>
              <div className='animate-low-bounce'>
                <img src={logo} className='w-48 h-48 shadow-xl shadow-slate-800/40' />
              </div>
              <Progress className="max-w-xl h-12" value={startupProgress.Value}></Progress>
              <p className="text-white">{startupProgress.Text}</p>
            </div>
          </>
          :
          <div className='flex flex-row h-svh'>
            <Sidebar />
            <div className='flex flex-col grow'>
              <Navbar />
              <div className='grow p-12 flex flex-col overflow-hidden'>
                <Outlet />
              </div>
              <div className='flex flex-row justify-end gap-2 px-5 py-2 bg-white dark:bg-zinc-800'>
                <div className='text-xs text-gray-500'>Mobiflight 2024</div>
                <div className='text-xs text-gray-500'>Version 1.0.0</div>
              </div>
            </div>
          </div>
      }
    </>
  )
}

export default App
