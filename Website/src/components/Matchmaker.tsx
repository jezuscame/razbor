// React modules
import {useCookies} from 'react-cookie'
import { useState, useEffect } from 'react'
// Application components
import { GetRequest } from '../Functions/GetRequest'
import { PostRequest } from '../Functions/PostRequest'
// Styling
import '../Styling/Matchmaker.css'

const Matchmaker = () => {
    const [dataArray, setDataArray] = useState<Array<any>>([])

    const [currentIndex, setCurrentIndex] = useState(0)

    const [cookies, setCookie, removeCookie] = useCookies(['AuthToken'])

    const [matchmakerDataLoading, setMatchmakerDataLoading] = useState(true)

    // Get request for possible matches
    useEffect(() => {
        GetRequest('matches', [], cookies.AuthToken)
        .then((response) => {
            const dataArray = response as Array<any>
            // Temp
            console.log(dataArray)
            setDataArray(dataArray)
        })
        .catch((error) => {
            console.error(error)
        })
        .finally(() => {
            setMatchmakerDataLoading(false)
        });
    }, [matchmakerDataLoading, cookies.AuthToken])

    const handleClickFight = async () => {
        if(dataArray.length === 0){
            setMatchmakerDataLoading(true)
        }
        else{
            const currentItem = dataArray[currentIndex]
            const username = currentItem.username;
            const response = await PostRequest('matches/match', [{ name: "user", value: username }], [], cookies.AuthToken);
            console.log(response)
            if(currentIndex === dataArray.length - 1){
                setMatchmakerDataLoading(true)
                setCurrentIndex(0)
            }
            else{
                setCurrentIndex(currentIndex + 1)
            }
        }
    }

    const handleClickFlee = () => {
        if(dataArray.length === 0){
            setMatchmakerDataLoading(true)
        }
        else
        {
            if(currentIndex === dataArray.length - 1){
                setMatchmakerDataLoading(true)
                setCurrentIndex(0)
            }
            else{
                setCurrentIndex(currentIndex + 1)
            }
        }
    }

    return (
        <div className="matchmaker">
            <div className="card">
                {dataArray.length === 0 ? (
                    <p>No more matches found.</p>
                ) : (
                    dataArray.map((user, index) => (
                    <div key={index}>
                        {index === currentIndex && (
                        <>
                            <div className="picture-container">
                            <img src={user.primaryPicture} alt="Picture of your fight buddy" />
                            </div>
                            <div className="swipe-info">
                            <h1>{user.username}</h1>
                            <h4>
                                Weight: {user.weight} kg⠀⠀⠀⠀⠀Height: {user.height} cm
                            </h4>
                            </div>
                        </>
                        )}
                    </div>
                    ))
                )}
            </div>
            <div className="button-container">
                <button className="primary-button" onClick={handleClickFlee}>
                    {"Flee"}
                </button>
                <button className="primary-button" onClick={handleClickFight}>
                    {"Fight"}
                </button>
            </div>
        </div>
    )
}
export default Matchmaker