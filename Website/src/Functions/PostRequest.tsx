import config from './config';

interface QueryParams {
  name: string;
  value: any;
}

interface BodyValues {
  name: string;
  value: any;
}

const ConvertBodyValuesToJSON = (body: BodyValues[]): string => {
  var text = body.map(({name, value}) => ` "${name}": ${JSON.stringify(value)}`).join(",\n")
  return "{\n" + text + "\n}"
}

// Todo: figure out how to use useNavigate inside this func without breaking hook rules
const PostRequest = async (childUrl: string, queryParams: QueryParams[], body: BodyValues[], authToken?: string): Promise<any> => {
  var queryString;
  if(queryParams.length > 0){
    queryString = queryParams
      .map(({ name, value }) => `${encodeURIComponent(name)}=${encodeURIComponent(value)}`)
      .join('&')
  }
  else{
    queryString = null
  }
  
  const jsonBody = ConvertBodyValuesToJSON(body)

  const headers: {[key: string]: string} = {
    'accept': 'text/plain',
    'content-type': 'application/json'      
  }
  
  if(authToken){
    headers['authorization'] = `Bearer ${authToken}`
  }

  var data;

  if(queryString != null){
    data = await fetch(`${config.apiBasePath}${childUrl}?${queryString}`, {
      method: 'POST',
      headers: headers,
      body: jsonBody
    });
  }
  else{
    data = await fetch(`${config.apiBasePath}${childUrl}`, {
      method: 'POST',
      headers: headers,
      body: jsonBody
    });
  }

  if(data.status === 401){
    // If the user is not authenticated, they will be
    // redirected to login page
    console.log("Unauthorized request at " + childUrl)
    window.location.href="/login"
  }

  const jsonData = await data.json();
  return jsonData;
};

export { PostRequest };