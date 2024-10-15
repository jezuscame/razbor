// React modules
import { useState } from 'react'
import { useCookies } from "react-cookie"
import { useNavigate } from 'react-router-dom'
// Application components
import Nav from '../components/Nav'
// Functions
import { PostRequest } from '../Functions/PostRequest'
import ValidateInput from '../Functions/InputValidation'
// Styling
import '../Styling/Registration.css'

const Registration = () => {
    const [ cookies, setCookie, removeCookie] = useCookies(['AuthToken'])

    const navigate = useNavigate()

    const [registrationFormData, setRegistrationFormData] = useState({
        username: "",
        password: "",
        confirmPassword: "",
        email: "",
        firstName: "",
        lastName: "",
        userGender: "", // Man - 0, woman - 1, other - 2
        birthday: "",
        weight: "",
        height: "",
        selectedSong: "",
        description: "",
        picture: "https://vignette.wikia.nocookie.net/villains/images/7/7b/Tylerbetterpicture.jpg/revision/latest?cb=20170410170911"
    })

    const [registrationFormValid, setRegistrationFormValid] = useState({
        username: false,
        password: false,
        confirmPassword: false,
        email: false,
        firstName: false,
        lastName: false,
        userGender: false, // Man - 0, woman - 1, other - 2
        birthday: false,
        weight: false,
        height: false,
        picture: true
    })

    const [errorMessage, setErrorMessage] = useState(""); 

    const handleRegistrationFormSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        // Stops the page from reseting once form is submited, needed for the user to see errors and make changes
        event.preventDefault()

        // Checks if all of the input fields are validated
        const isRegistrationFormValid = Object.values(registrationFormValid).every((value) => value === true)

        if (!isRegistrationFormValid) {
            setErrorMessage('Please check your form once again and make sure all of its fields are filled out and valid');
            return;
        }
        
        // Forms the body for the registration POST request
        const registrationRequestBody = [
            { name: "username", value: registrationFormData.username },
            { name: "password", value: registrationFormData.password },
            { name: "email", value: registrationFormData.email },
            { name: "firstName", value: registrationFormData.firstName },
            { name: "lastName", value: registrationFormData.lastName },
            { name: "userGender", value: parseInt(registrationFormData.userGender) },
            { name: "birthday", value: registrationFormData.birthday },
            { name: "height", value: parseInt(registrationFormData.height) },
            { name: "weight", value: parseInt(registrationFormData.weight) },
            { name: "selectedSong", value: registrationFormData.selectedSong },
            { name: "description", value: registrationFormData.description },
            { name: "primaryPicture", value: registrationFormData.picture }
        ]

        // Response stores the JSON body containing the authentication token
        const response = await PostRequest('register', [], registrationRequestBody)
        
        if(response.status !== 200){
            setErrorMessage("We're sorry, there has been an error with your registration request");
        }

        // The token is retrieved and saved as a cookie
        const token = response.token
        setCookie('AuthToken', token)
        
        // The user is redirected to the home page
        navigate('/')
    }

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.type === "checkbox" ? event.target.checked : event.target.value
        const name = event.target.name

        // Saves the new value of the input
        setRegistrationFormData((prevState) => ({
            ...prevState,
            [name] : value
        }))

        switch(name){
            case "confirmPassword": {
                // Checks if the confirm password field is the same as password
                setRegistrationFormValid((prevState) => ({
                    ...prevState,
                    [name] : (registrationFormData.password === value)
                }));
                break;
            };
            case "userGender": {
                // As long as the user has selected a value, it is valid
                setRegistrationFormValid((prevState) => ({
                    ...prevState,
                    [name] : true
                }));
                break;
            };
            case "selectedSong":
            case "description": 
            case "picture": {
                break; // Not neccesary values are not checked, picture stays true
            }
            default: {
                // Checks if the new value of the input is valid
                setRegistrationFormValid((prevState) => ({
                    ...prevState,
                    [name] : ValidateInput(name,value)
                }));
            };
        }        
    }
     
    return (
        <>
        <Nav 
            authToken={false}
            minimal={true}
            setShowModal={() => {}} // Just an empty function as login module has no point here
            showModal={false}
        />
        <div className="registration">
            <h2>CREATE ACCOUNT</h2>
            <form onSubmit={handleRegistrationFormSubmit}>
                <section>
                    <label htmlFor="username">Username *</label>
                    <input
                        id="username"
                        type="text"
                        name="username"
                        placeholder="Username"
                        required={true}
                        value={registrationFormData.username}
                        onChange={handleInputChange}
                    />
                    {!registrationFormValid.username && registrationFormData.username !== "" && <p className='inputErrorMessage'>Username invalid (alfanumeric characters only)</p>}

                    <label htmlFor="Password">Password *</label>
                    <input
                        id="password"
                        type="password"
                        name="password"
                        placeholder="Password"
                        required={true}
                        value={registrationFormData.password}
                        onChange={handleInputChange}
                    />
                    {!registrationFormValid.password && registrationFormData.password !== "" && <p className='inputErrorMessage'>Password invalid (should contain at least 5 symbols)</p>}
                    <input
                        id="confirmPassword"
                        type="password"
                        name="confirmPassword"
                        placeholder="Confirm Password"
                        required={true}
                        value={registrationFormData.confirmPassword}
                        onChange={handleInputChange}
                    />
                    {!registrationFormValid.confirmPassword && registrationFormData.confirmPassword !== "" && <p className='inputErrorMessage'>Passwords must match</p>}
                    
                    <label htmlFor="email">Email *</label>
                    <input
                        id="email"
                        type="email"
                        name="email"
                        placeholder="Email"
                        required={true}
                        value={registrationFormData.email}
                        onChange={handleInputChange}
                    />
                    {!registrationFormValid.email && registrationFormData.email !== "" && <p className='inputErrorMessage'>Email invalid (please enter a valid email)</p>}

                    <label htmlFor="firstName">First Name *</label>
                    <input
                        id="firstName"
                        type="text"
                        name="firstName"
                        placeholder="First Name"
                        required={true}
                        value={registrationFormData.firstName}
                        onChange={handleInputChange}
                    />
                    {!registrationFormValid.firstName && registrationFormData.firstName !== "" && <p className='inputErrorMessage'>First name invalid (english letters only)</p>}

                    <label htmlFor="lastName">Last Name *</label>
                    <input
                        id="lastName"
                        type="text"
                        name="lastName"
                        placeholder="Last Name"
                        required={true}
                        value={registrationFormData.lastName}
                        onChange={handleInputChange}
                    />
                    {!registrationFormValid.lastName && registrationFormData.lastName !== "" && <p className='inputErrorMessage'>Last name invalid (english letters only)</p>}

                    <label>Gender *</label>
                    <div className="multiple-input-container">                    
                        <input
                            id="man-userGender"
                            type="radio"
                            name="userGender"
                            value="0"
                            onChange={handleInputChange}
                            checked={registrationFormData.userGender === "0"}
                        />
                        <label htmlFor="man-userGender">Man</label>                    
                        <input
                            id="woman-userGender"
                            type="radio"
                            name="userGender"
                            value="1"
                            onChange={handleInputChange}
                            checked={registrationFormData.userGender === "1"}
                        />
                        <label htmlFor="woman-userGender">Woman</label>                    
                        <input
                            id="other-userGender"
                            type="radio"
                            name="userGender"
                            value="2"
                            onChange={handleInputChange}
                            checked={registrationFormData.userGender === "2"}
                        />
                        <label htmlFor="other-userGender">Other</label>
                    </div>

                    <label>Date of birth *</label>
                    <input
                        id="birthday"
                        type="date"
                        name="birthday"
                        required={true}
                        value={registrationFormData.birthday}
                        onChange={handleInputChange}
                    />
                    {!registrationFormValid.birthday && registrationFormData.birthday !== "" && <p className='inputErrorMessage'>Date invalid</p>}

                    <label>Stats *</label>
                    <div className="multiple-input-container">
                    <label htmlFor="weight">Weight</label>
                    <input
                        id="weight"
                        type="number"
                        name="weight"
                        placeholder="KG"
                        required={true}
                        value={registrationFormData.weight}
                        onChange={handleInputChange}
                    />                                        
                    <label htmlFor="height">Height</label>
                    <input
                        id="height"
                        type="number"
                        name="height"
                        placeholder="CM"
                        required={true}
                        value={registrationFormData.height}
                        onChange={handleInputChange}
                    />                   
                    </div>
                    {!registrationFormValid.weight && registrationFormData.weight !== "" && <p className='inputErrorMessage'>Please enter a valid weight in KG</p>}
                    {!registrationFormValid.height && registrationFormData.height !== "" && <p className='inputErrorMessage'>Please enter a valid height in CM</p>} 

                    <label htmlFor="description">About me</label>
                    <input
                        id="description"
                        type="text"
                        name="description"
                        placeholder="I can't sleep"
                        value={registrationFormData.description}
                        onChange={handleInputChange}
                    />

                    <label htmlFor="selectedSong">Fighting song</label>
                    <input
                        id="selectedSong"
                        type="url"
                        name="selectedSong"
                        placeholder="Paste song link here"
                        value={registrationFormData.selectedSong}
                        onChange={handleInputChange}
                    />

                    <p>Fields marked with * must be filled out</p>

                    <input type="submit" value="Register"/>
                    {errorMessage && <div className="errorMessage">{errorMessage}</div>}
                </section>
                

                <section>
                    <label htmlFor="picture">Mugshot *</label>
                    <input
                        type="url"
                        name="picture"
                        id="picture"
                        required={true}
                        value={registrationFormData.picture}
                        onChange={handleInputChange}
                    />
                    <div className="picture-container">
                        <p>Disclaimer: if you do not use a square image, your pricture will show up streched here and on other pages.</p>
                        {registrationFormData.picture && <img src={registrationFormData.picture} alt="Profile picture preview"/>}
                    </div>
                </section>
            </form>
        </div>
        </>
        
    )
}
export default Registration