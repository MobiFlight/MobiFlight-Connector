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

interface ConfigLoadedEvent {
  payload : {
    FileName : string
    ConfigItems: Types.IConfigItem[]  
  } 
}

interface EventMessage {
  key: string,
  payload: any
}

interface StatusBarUpdate {
  Text: string
  Value: number
}


function App() {
  const navigate = useNavigate();
  const { setItems, updateItem } = useConfigStore()

  const handleMessage = (message: any) => {
    var eventData = JSON.parse(message.data) as EventMessage
    console.log("Handle Message")
    console.log(eventData.key)

    if(eventData.key == "config.update") { 
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

    if(eventData.key == "StatusBarUpdate") {
      const update = eventData.payload as StatusBarUpdate
      setStartupProgress(update)
    } 

    if(eventData.key === "ConfigFile") {
      setStartupProgress({ Value:100, Text: "Finished!" })
      const configFile = JSON.parse(message.data) as ConfigLoadedEvent
      console.log("Config File Loaded")
      setItems(configFile.payload.ConfigItems)
      navigate(`/projects/1`);
    }
  }

  const [queryParameters] = useSearchParams()
  const [startupProgress, setStartupProgress] = useState<StatusBarUpdate>({ Value:0, Text: "Starting..." })

  useEffect(() => {
    window.chrome?.webview?.addEventListener('message', handleMessage)
    return () => {
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

  useEffect(()=>{
    if (queryParameters.get("progress")=="100") setStartupProgress({ Value:100, Text: "Finished!" })
  }, [])

  
  return (
    <>
      {
        startupProgress.Value < 100
        ?
        <>
          <div className="fixed inset-0 bg-[url('/assets/background-nologo.jpg')]"></div>
          <div className='relative flex flex-col justify-center min-h-screen p-10 items-center gap-8'>
            <img src={logo} className='w-48 h-48'/>
            <Progress className="max-w-xl h-12" value={startupProgress.Value}></Progress>
            <p className="text-white">{startupProgress.Text}</p>
          </div>
        </>
        :
        <div className='flex flex-row min-h-screen h-72 bg-gradient-to-r from-primary-600 via-blue-600 to-white-600 "'>
          <Sidebar />
          <div className='w-full'>
            <Navbar />
            <div className='p-12 overflow-hidden'>
              <div className='h-full p-4 overflow-auto'>
                <Outlet />
              </div>
            </div>
          </div>
        </div>
      }
    </>
  )
}

export default App
