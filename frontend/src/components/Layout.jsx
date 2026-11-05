import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'

function Layout() {
  const { user, logout } = useAuth()

  return (
    <div className="app">
      <header className="topbar">
        <NavLink to="/" className="brand">
          <img src="/d20.svg" alt="" width="28" height="28" />
          PF2e Campaign Tracker
        </NavLink>

        {user && (
          <nav>
            <NavLink to="/characters">Characters</NavLink>
            <NavLink to="/campaigns">Campaigns</NavLink>
            <NavLink to="/encounters">Initiative</NavLink>
            <NavLink to="/expenses">Expenses</NavLink>
            <NavLink to="/expenses/charts">Charts</NavLink>
          </nav>
        )}

        {user && (
          <div className="user">
            <span>{user.displayName}</span>
            <button className="link" onClick={logout}>Log out</button>
          </div>
        )}
      </header>

      <main className="content">
        <Outlet />
      </main>
    </div>
  )
}

export default Layout
