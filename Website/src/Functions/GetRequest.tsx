import config from './config';

interface QueryParams {
  name: string;
  value: string;
}

const GetRequest = async (childUrl: string, queryParams: QueryParams[], authToken?: string): Promise<any> => {
    const headers: {[key: string]: string} = {
      'accept': 'text/plain',
      'content-type': 'application/json'      
    }
    if(authToken){
      headers['authorization'] = `Bearer ${authToken}`
    }

    const queryString = queryParams
      .map(({ name, value }) => `${encodeURIComponent(name)}=${encodeURIComponent(value)}`)
      .join('&');
    
    const data = await fetch(`${config.apiBasePath}${childUrl}?${queryString}`, {
      method: 'GET',
      headers: headers
    });

    if(data.status === 401){
      // If the user is not authenticated, they will be
      // redirected to login page
      console.log("Unauthorized request at " + childUrl)
      window.location.href="/login"
    }

    const jsonData = await data.json();
    return jsonData;
  };
  
  export { GetRequest };