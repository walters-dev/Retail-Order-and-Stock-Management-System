let currentAccessToken = null;

export function setAccessToken(token){
    currentAccessToken = token;
}

export function getAccessToken(){
    return currentAccessToken;
}

export function clearAccessToken(){
    currentAccessToken = null;
}