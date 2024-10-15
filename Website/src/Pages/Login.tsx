// React modules
import { useState } from 'react'
import { useCookies } from 'react-cookie'
import { useNavigate } from 'react-router-dom'
// Application components
import Nav from '../components/Nav'
// Functions
import { PostRequest } from '../Functions/PostRequest'
// Styling
import '../Styling/Login.css'

// TODO decide if this page will be replaced by the use of the auth modal
// if not - needs input validation and feedback
const Login = () => {
    const [ cookies, setCookie ] = useCookies(['AuthToken'])

    const navigate = useNavigate();

    const [error, setError] = useState<string | null>(null)

    const [formData, setFormData] = useState({
        username: "",
        password: ""
    })    
    
    const handleChange = (e: any ) => {
        const value = e.target.value
        const name = e.target.name
        console.log('value' + value, 'name' + name)
    
        setFormData((prevState) => ({
            ...prevState,
            [name] : value
        }))
    }

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault()
        try{
            const requestBody = [
                { name: "username", value: formData.username },
                { name: "password", value: formData.password }
            ]

            // Response stores the JSON body containing the authentication token
            const response = await PostRequest('login', [], requestBody)
            const token = response.token
            console.log(token)
            console.log(formData.username)
            setCookie('AuthToken', token)
            navigate(-1)
            console.log("veikia")
        }
        catch(error){
            setError('An error occurred while logging in. Please try again.')
            // Temp
            console.log("gaidynas")
        }
    }

    return (
        <>
        <Nav 
            authToken={false}
            minimal={true}
            setShowModal={() => {}} // Just an empty func
            showModal={false}
        />
        <div className="login">

            <h2>LOG INTO YOUR ACCOUNT</h2>
            {error
                ? <p className="error"> An error occurred while logging in. Please try again </p>
                : <p>There has been an authorization error while processing your request. Please log in again. </p>
                }
            <form onSubmit={handleSubmit}>

                <label htmlFor="username">Username</label>
                <input
                    id="username"
                    type="text"
                    name="username"
                    placeholder="Username"
                    required={true}
                    value={formData.username}
                    onChange={handleChange}
                />

                <label htmlFor="Password">Password</label>
                <input
                    id="password"
                    type="password"
                    name="password"
                    placeholder="Password"
                    required={true}
                    value={formData.password}
                    onChange={handleChange}
                />
                <input 
                    type="submit"
                    value="Login"
                />
            </form>
        </div>
        
        </>
    )
}
export default Login