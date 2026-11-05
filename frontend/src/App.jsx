import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import ProtectedRoute from './components/ProtectedRoute'
import { AuthProvider } from './hooks/useAuth'
import CampaignsPage from './pages/CampaignsPage'
import CharacterDetailPage from './pages/CharacterDetailPage'
import CharacterFormPage from './pages/CharacterFormPage'
import CharactersPage from './pages/CharactersPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />

            <Route element={<ProtectedRoute />}>
              <Route path="/" element={<Navigate to="/characters" replace />} />
              <Route path="/characters" element={<CharactersPage />} />
              <Route path="/characters/new" element={<CharacterFormPage />} />
              <Route path="/characters/:id" element={<CharacterDetailPage />} />
              <Route path="/characters/:id/edit" element={<CharacterFormPage />} />
              <Route path="/campaigns" element={<CampaignsPage />} />
            </Route>

            <Route path="*" element={<Navigate to="/" replace />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
