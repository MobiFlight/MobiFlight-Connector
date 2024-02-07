import './App.css'
import { Outlet } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Navbar from './components/Navbar';

// import i18n (needs to be bundled ;)) 
import './i18n';

function App() {
  return (
    <>
      <div className='flex flex-row min-h-screen h-72 bg-gradient-to-r from-primary-600 via-blue-600 to-white-600"'>
        <Sidebar />
        <div className='w-full'>
          <Navbar />
          <div className='p-12'>
          <Outlet />
          </div>
        </div>
      </div>
    </>
  )
}

export default App
