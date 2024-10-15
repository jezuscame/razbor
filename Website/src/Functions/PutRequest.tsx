import config from './config';

/* Taken out for now,
might introduce later if needed
interface QueryParams {
    name: string;
    value: any;
  } */

interface BodyValues {
  name: string;
  value: any;
}

const ConvertBodyValuesToJSON = (body: BodyValues[]): string => {
  var text = body.map(({name, value}) => ` "${name}": ${JSON.stringify(value)}`).join(",\n")
  return "{\n" + text + "\n}"
}

// Todo: figure out how to use useNavigate inside this func without breaking hook rules
const PutRequest = async (childUrl: string, body: BodyValues[], authToken?: string): Promise<any> => {
  const jsonBody = ConvertBodyValuesToJSON(body)

  const headers: {[key: string]: string} = {
    'accept': 'text/plain',
    'content-type': 'application/json'      
  }
  if(authToken){
    headers['authorization'] = `Bearer ${authToken}`
  }

  const data = await fetch(`${config.apiBasePath}${childUrl}`, {
    method: 'PUT',
    headers: headers,
    body: jsonBody
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

export { PutRequest };