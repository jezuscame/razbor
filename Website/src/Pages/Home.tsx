// React modules
import { useState } from 'react'
import {useCookies} from 'react-cookie'
import { useNavigate } from 'react-router-dom'
// Application components
import Nav from '../components/Nav'
import AuthModal from '../components/AuthModal'
// Styling
import '../Styling/Home.css'

const Home = () => {
    const [cookies, setCookie, removeCookie] = useCookies(['AuthToken'])

    const navigate = useNavigate()

    const [showModal, setShowModal] = useState(false)
    
    const authToken = cookies.AuthToken

    const handleClick = () => {        
        authToken ? navigate('/dashboard') : navigate('/Registration')
    }

    return (
        <>
        <Nav authToken={authToken} 
            minimal={false} 
            setShowModal={setShowModal} 
            showModal={showModal}
        />
        {showModal &&(
            <AuthModal setShowModal={setShowModal}/>
        )}
        <div className="home">
            <h1 className='primary-title'>"How much can you know about yourself if you've never been in a fight?"</h1>
            <h2 className='secondary-title'>- Chuck Palahniuk, Fight Club</h2>

            <button className="primary-button" onClick={handleClick}>
                {authToken ? 'Fight' : 'Create Account'}
            </button>
        </div>
        </>
    )
}
export default Home