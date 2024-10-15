// React modules
import { useState } from "react"
import {useCookies} from "react-cookie"
// Functions
import { PostRequest } from '../Functions/PostRequest'
// Styling
import '../Styling/AuthModal.css'


const AuthModal = ({setShowModal} : {setShowModal:any}) => {
    const [username, setUsername] = useState(null)
    const [password, setPassword] = useState(null)
    const [error, setError] = useState<string | null>(null)
    const [cookies, setCookie, removeCookie] = useCookies(['AuthToken'])

    const handleCloseClick = () => {
        setShowModal(false)
    }
    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => { 
        event.preventDefault()
        try{
            const requestBody = [
                { name: "username", value: username },
                { name: "password", value: password }
            ]
    
            // Response stores the JSON body containing the authentication token
            const response = await PostRequest('login', [], requestBody)

            //Temp
            console.log(response);

            setCookie('AuthToken', response.token)

            setShowModal(false)            
        }
        catch(error){
            setError('An error occurred while logging in. Please try again.')
            // Temp
            console.log(error)
        }
    }

    return (
        <div className="auth-modal" onClick={handleCloseClick}>
            <div className="input-container" onClick={(event) => event.stopPropagation()}>
                <button className="close-button" onClick={handleCloseClick}>
                    X
                </button>
                <h2>LOG IN</h2>
                {error
                    ? <p className="error"> An error occurred while logging in. Please try again. </p>
                    : <p>By logging in you agree to our privacy policy and rules of conduct </p>
                }
                <form onSubmit={handleSubmit}>
                    <input 
                        type="text"
                        id="username"
                        name="username"
                        placeholder="Username"
                        required={true}
                        onChange={(e:any) => setUsername(e.target.value)}
                    />
                    <input 
                        type="password"
                        id="password"
                        name="password"
                        placeholder="Password"
                        required={true}
                        onChange={(e:any) => setPassword(e.target.value)}
                    />
                    <input className="login-button" type="submit" value="Log in"/>
                </form>
            </div>          
        </div>
    )
}
export default AuthModal