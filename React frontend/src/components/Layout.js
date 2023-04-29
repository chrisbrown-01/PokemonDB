import { Outlet, Link } from "react-router-dom";
import SidePanel from './SidePanel';
import './Layout.css'
import { NavLink } from 'react-router-dom';
import '../custom.css';

const Layout = () => {
    return (
        <div>
            <nav className="navbar-layout">
                <ul>
                    <Link to="/">
                        <img src="/Logo.svg" alt="Pokemon DB Logo" />
                    </Link>
                    <li>
                        <NavLink activeclassname="active" to="/profiles">Pokemon Profiles</NavLink>
                    </li>
                    <li>
                        <NavLink activeclassname="active" to="/favourites">Favourites</NavLink>
                    </li>
                    <li>
                        <NavLink activeclassname="active" to="/advancedsearch">Advanced Search</NavLink>
                    </li>
                </ul>
            </nav>

            <div className="content-layout">
                <div className="content">
                    <Outlet />
                </div>
                <div className="sidepanel">
                    <SidePanel />
                </div>
            </div>
            <footer>
                Designed by Chris Brown in April 2023 -
                <a href="mailto:chrisbrown1@hotmail.ca"> Email </a> |
                <a href="https://www.linkedin.com/in/chrisbrown1000/" target="_blank" rel="noopener noreferrer"> LinkedIn </a> |
                <a href="https://github.com/ConkyTheGreat" target="_blank" rel="noopener noreferrer"> GitHub </a>
            </footer>
        </div>
    )
};

export default Layout;