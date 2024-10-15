// React modules
import { useCookies } from "react-cookie"
import { useNavigate } from 'react-router-dom'
// Styling
import '../Styling/Navbar.css'
// Elements
import logo from '../images/logo.png'

const Nav = ({ authToken, minimal, setShowModal, showModal } : {authToken: any, minimal:any, setShowModal:any, showModal:any}) => {
    const navigate = useNavigate()

    const [ cookies, setCookie, removeCookie] = useCookies(['AuthToken'])
    
    const handleLogin = () => {
        setShowModal(true)
    }

    const handleLogout = () => {
        removeCookie('AuthToken', authToken)
        window.location.href = '/'
    }

    const handleNavigate = (event: React.MouseEvent<HTMLButtonElement>) => {
        // Use the id of a button to navigate to the desired page
        navigate(event.currentTarget.id);
    }

    return (
        <nav>
            <section className="navigation">
                <button id="/" className="logo-button" onClick={handleNavigate}>
                    <img className="logo" src={logo} />
                </button>
                <button id="/Dashboard" className="nav-button" onClick={authToken ? handleNavigate : handleLogin}>
                    Matching
                </button>
                <button id="/Profile" className="nav-button" onClick={authToken ? handleNavigate : handleLogin}>
                    Profile
                </button>             
            </section>
            {!authToken && 
                <section className="authentication">
                    <button className="nav-button" onClick={handleLogin} disabled={showModal}>
                        Log in
                    </button>
                </section>
            }
            {authToken && 
                <section className="authentication">
                    <button className="nav-button" onClick={handleLogout} disabled={showModal}>
                        Log out
                    </button> 
                </section>
            }
        </nav>
    )
}
export default Nav