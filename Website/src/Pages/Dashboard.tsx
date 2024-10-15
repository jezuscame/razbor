// React modules
import {useCookies} from 'react-cookie'
import { useState, useEffect } from 'react'
// Application components
import Nav from '../components/Nav'
import ChatContainer from '../components/ChatContainer'
import Matchmaker from '../components/Matchmaker'
import ChatList from '../components/ChatList'
// Styling
import '../Styling/Dashboard.css'

const Dashboard = () => {
  const [chatListDataLoading, setChatListDataLoading] = useState(true)

  const [cookies, setCookie, removeCookie] = useCookies(['AuthToken'])

  const authToken = cookies.AuthToken

  return (
    <>
    <Nav 
      authToken={authToken}
        minimal={true}
        setShowModal={() => {}} // Just an empty func
        showModal={false}
    />
    <div className="dashboard">
      <section className="chat-list-section">
        <ChatList
        chatListDataLoading={chatListDataLoading}
        setChatListDataLoading={setChatListDataLoading}
        />
      </section>
      <section className="matchmaker-section">
        <Matchmaker/>
      </section>
    </div>
    </>
  )
}
export default Dashboard