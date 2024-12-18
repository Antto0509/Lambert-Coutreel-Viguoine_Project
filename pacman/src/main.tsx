import { BrowserRouter } from 'react-router-dom'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import Header from './components/header.tsx'

createRoot(document.getElementById('root')!).render(
  <BrowserRouter>
    <Header />
    <App />
  </BrowserRouter>
)
