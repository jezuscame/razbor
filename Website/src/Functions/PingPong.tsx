import { useState, useEffect } from 'react';
import {GetRequest} from './GetRequest';

function PingPong() {
    const [isPing, setIsPing] = useState(true);
    const [serverResponse, setServerResponse] = useState('');

    const api = async () => {
      const queryParams = [ { name: 'Message', value: 'sdas'} ];
      const response = await GetRequest('ping', queryParams)
      console.log(response.message);
      setServerResponse(response.message as string);
      setIsPing(false);
    };

    useEffect(() => {
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