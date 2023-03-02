let responseJson = pm.response.json();
let jwtToken = responseJson.jwtToken;
pm.environment.set("jwtToken", jwtToken);
