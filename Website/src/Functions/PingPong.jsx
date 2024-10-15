import { useState, useEffect } from 'react';

function PingPong() {
    const [isPing, setIsPing] = useState(true);
    const [serverResponse, setServerResponse] = useState('');
  
  
    useEffect(() => {
        const api = async () => {
          const data = await fetch("http://localhost:44393/ping?Message=rimta", {
            method: "GET"
          });
          const jsonData = await data.json();
          setServerResponse(jsonData.results);
        };
    
        api();
      }, []);
  

    return (
      <>
        <p>{isPing? "Ping!" : "Pong!"}</p>
        <p>Server response: {serverResponse}</p>
      </>
    );
  }
  
  export default PingPong;