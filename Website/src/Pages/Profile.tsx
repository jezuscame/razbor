// React modules
import { useState } from 'react'
import { useEffect } from 'react'
import { useCookies } from "react-cookie"
// Application components
import Nav from '../components/Nav'
// Functions
import { PutRequest } from '../Functions/PutRequest'
import { GetRequest } from '../Functions/GetRequest'
import ValidateInput from '../Functions/InputValidation'
// Styling
import '../Styling/Profile.css'

const Profile = () => {
    const [cookies] = useCookies(['AuthToken'])

    const [editMode, setEditMode] = useState(false)

    const [profileFormData, setProfileFormData] = useState({
        username: "",
        email: "",
        firstName: "",
        lastName: "",
        userGender: "", // Man - 0, woman - 1, other - 2
        birthday: "",
        weight: "",
        height: "",
        selectedSong: "",
        description: "",
        picture: ""
    })

    const [profileFormValid, setProfileFormValid] = useState({
        email: false,
        firstName: false,
        lastName: false,
        userGender: false, // Man - 0, woman - 1, other - 2
        birthday: false,
        weight: false,
        height: false,
        picture: true
    })

    const [profileDataLoading, setProfileDataLoading] = useState(true);

    useEffect(() => {
        // Makes the GET request to fetch the user's information
        GetRequest('user/profile', [], cookies.AuthToken).then((response) => {
            //TODO maybe display authModal instead of redirecting to login page?

            // Updates the profileFormData state with the retrieved data
            setProfileFormData({
                username: response.username,
                email: response.email,
                firstName: response.firstName,
                lastName: response.lastName,
                userGender: response.userGender.toString(),
                birthday: response.birthday.split("T")[0],
                weight: response.weight.toString(),
                height: response.height.toString(),
                selectedSong: response.selectedSong,
                description: response.description,
                picture: response.primaryPicture,
            });
        
            // Set the profileFormValid state to true for all fields
            setProfileFormValid({
                email: true,
                firstName: true,
                lastName: true,
                userGender: true,
                birthday: true,
                weight: true,
                height: true,
                picture: true,
            });            
        })
        .finally(() => {
            setProfileDataLoading(false)
        });
    }, [profileDataLoading]);

    const [errorMessage, setErrorMessage] = useState("");

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.value
        const name = event.target.name

        // Saves the new value of the input
        setProfileFormData((prevState) => ({
            ...prevState,
            [name] : value
        }))

        switch(name){
            case "userGender": {
                // As long as the user has selected a value, it is valid
                setProfileFormValid((prevState) => ({
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
                setProfileFormValid((prevState) => ({
                    ...prevState,
                    [name] : ValidateInput(name,value)
                }));
            };
        }        
    }

    const handleEditClick = () => {
        setEditMode(true)
    }

    const handleCancelClick = () => {
        setProfileDataLoading(true)
        setEditMode(false)
    }

    const handleProfileFormSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        // Stops the page from reseting once form is submited, needed for the user to see errors and make changes
        event.preventDefault()

        // Checks if all of the input fields are validated
        const isProfileFormValid = Object.values(profileFormValid).every((value) => value === true)

        if (!isProfileFormValid) {
            setErrorMessage('Please check your form once again and make sure all of its fields are filled out and valid');
            return;
        }

        // Forms the body for the registration POST request
        const profileRequestBody = [
            { name: "email", value: profileFormData.email },
            { name: "firstName", value: profileFormData.firstName },
            { name: "lastName", value: profileFormData.lastName },
            { name: "birthday", value: profileFormData.birthday },
            { name: "height", value: parseInt(profileFormData.height) },
            { name: "weight", value: parseInt(profileFormData.weight) },
            { name: "selectedSong", value: profileFormData.selectedSong },
            { name: "description", value: profileFormData.description },
            { name: "primaryPicture", value: profileFormData.picture }
        ]

        // Response stores the JSON body containing the authentication token
        const response = await PutRequest('user/update', profileRequestBody, cookies.AuthToken)

        if(response !== 200){
            setErrorMessage("We're sorry, there has been an error with your request");
        }
        else
        {
            setEditMode(false)
            setProfileDataLoading(true)
            setErrorMessage("");
        }
    }

    return (
        <>
        <Nav 
            authToken={cookies.AuthToken} 
            minimal={false} 
            setShowModal={()=>{}} 
            showModal={false}
        />

        <div className="profile">
            <h2>YOUR PROFILE</h2>
            <form onSubmit={handleProfileFormSubmit}>
                <section>
                    <label htmlFor="username">Username</label>
                    <input
                        id="username"
                        type="text"
                        name="username"
                        placeholder="Username"
                        required={true}
                        value={profileFormData.username}
                        disabled
                    />
                    
                    <label htmlFor="email">Email *</label>
                    <input
                        id="email"
                        type="email"
                        name="email"
                        placeholder="Email"
                        required={true}
                        value={profileFormData.email}
                        onChange={handleInputChange}
                        disabled={!editMode}
                    />
                    {!profileFormValid.email && profileFormData.email !== "" && <p className='inputErrorMessage'>Email invalid (please enter a valid email)</p>}

                    <label htmlFor="firstName">First Name *</label>
                    <input
                        id="firstName"
                        type="text"
                        name="firstName"
                        placeholder="First Name"
                        required={true}
                        value={profileFormData.firstName}
                        onChange={handleInputChange}
                        disabled={!editMode}
                    />
                    {!profileFormValid.firstName && profileFormData.firstName !== "" && <p className='inputErrorMessage'>First name invalid (english letters only)</p>}

                    <label htmlFor="lastName">Last Name *</label>
                    <input
                        id="lastName"
                        type="text"
                        name="lastName"
                        placeholder="Last Name"
                        required={true}
                        value={profileFormData.lastName}
                        onChange={handleInputChange}
                        disabled={!editMode}
                    />
                    {!profileFormValid.lastName && profileFormData.lastName !== "" && <p className='inputErrorMessage'>Last name invalid (english letters only)</p>}

                    <label>Gender</label>
                    <div className="multiple-input-container">                    
                        <input
                            id="man-userGender"
                            type="radio"
                            name="userGender"
                            value="0"
                            onChange={handleInputChange}
                            checked={profileFormData.userGender === "0"}
                            disabled={true}
                        />
                        <label htmlFor="man-userGender">Man</label>                    
                        <input
                            id="woman-userGender"
                            type="radio"
                            name="userGender"
                            value="1"
                            onChange={handleInputChange}
                            checked={profileFormData.userGender === "1"}
                            disabled={true}
                        />
                        <label htmlFor="woman-userGender">Woman</label>                    
                        <input
                            id="other-userGender"
                            type="radio"
                            name="userGender"
                            value="2"
                            onChange={handleInputChange}
                            checked={profileFormData.userGender === "2"}
                            disabled={true}
                        />
                        <label htmlFor="other-userGender">Other</label>
                    </div>

                    <label>Date of birth *</label>
                    <input
                        id="birthday"
                        type="date"
                        name="birthday"
                        required={true}
                        value={profileFormData.birthday}
                        onChange={handleInputChange}
                        disabled={!editMode}
                    />
                    {!profileFormValid.birthday && profileFormData.birthday !== "" && <p className='inputErrorMessage'>Date invalid</p>}

                    <label>Stats *</label>
                        <div className="multiple-input-container">
                        <label htmlFor="weight">Weight</label>
                        <input
                            id="weight"
                            type="number"
                            name="weight"
                            placeholder="KG"
                            required={true}
                            value={profileFormData.weight}
                            onChange={handleInputChange}
                            disabled={!editMode}
                        />                                        
                        <label htmlFor="height">Height</label>
                        <input
                            id="height"
                            type="number"
                            name="height"
                            placeholder="CM"
                            required={true}
                            value={profileFormData.height}
                            onChange={handleInputChange}
                            disabled={!editMode}
                        />                   
                    </div>
                    {!profileFormValid.weight && profileFormData.weight !== "" && <p className='inputErrorMessage'>Please enter a valid weight in KG</p>}
                    {!profileFormValid.height && profileFormData.height !== "" && <p className='inputErrorMessage'>Please enter a valid height in CM</p>} 

                    <label htmlFor="description">About me</label>
                    <input
                        id="description"
                        type="text"
                        name="description"
                        placeholder="I can't sleep"
                        value={profileFormData.description}
                        onChange={handleInputChange}
                        disabled={!editMode}
                    />

                    <label htmlFor="selectedSong">Fighting song</label>
                    <input
                        id="selectedSong"
                        type="url"
                        name="selectedSong"
                        placeholder="Paste song link here"
                        value={profileFormData.selectedSong}
                        onChange={handleInputChange}
                        disabled={!editMode}
                    />

                    <p>Fields marked with * must be filled out before saving changes</p>
                    <div className='button-container'>
                        {!editMode ? (
                            <input 
                                type="button" 
                                value="Edit" 
                                onClick={handleEditClick}
                                disabled={editMode}
                            />
                        ) : (
                            <input 
                                type="button" 
                                value="Cancel" 
                                onClick={handleCancelClick}
                                disabled={!editMode}
                            />
                        )}
                        
                        <input 
                            type="submit" 
                            value="Save changes"
                            disabled={!editMode}
                        />
                    </div>
                    
                    {errorMessage && <div className="errorMessage">{errorMessage}</div>}
                </section>
                

                <section>
                    <label htmlFor="picture">Mugshot *</label>
                    <input
                        type="url"
                        name="picture"
                        id="picture"
                        required={true}
                        value={profileFormData.picture}
                        onChange={handleInputChange}
                        disabled={!editMode}
                    />
                    <div className="picture-container">
                        <p>Disclaimer: if you do not use a square image, your pricture will show up streched here and on other pages.</p>
                        {profileFormValid.picture && <img src={profileFormData.picture} alt="Profile picture preview"/>}
                    </div>
                </section>
            </form>
        </div>
        </>
    )
}
export default Profile