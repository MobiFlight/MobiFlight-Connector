import { useTranslation } from 'react-i18next';
import './App.css'
import { Link } from 'react-router-dom';
import { Projects } from './fixtures/projects'

function App() {
  const { t } = useTranslation();
  const projects = Projects

  return (
    <>
      <h1>{t('app.greeting')}</h1>
      <h2>Projects</h2>
      {
        projects.map(p => {
          return <div><Link key={p.filePath} to={`projects/${p.id}`}>
            <p>{p.name}</p>
            <p>Devices: {p.status.devices.count.toString()}</p>
            <p>{p.status.sim.name}: {p.status.sim.status}</p>
            </Link>
          </div>
        })
      }
    </>
  )
}

export default App
