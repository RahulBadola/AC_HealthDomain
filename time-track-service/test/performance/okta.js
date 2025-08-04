import http from 'k6/http';

const OKTA_DOMAIN = __ENV.OKTA_DOMAIN || 'assurecare.okta.com';
const OKTA_AUTHSERVER_ID = __ENV.OKTA_AUTHSERVER_ID || 'default';
const OKTA_CLIENT_ID = __ENV.OKTA_CLIENT_ID || '0oa1i0sajflBIKRIG0h8';
const OKTA_CLIENT_SECRET = __ENV.OKTA_CLIENT_SECRET || 'KlRnSeps5bdF3PG2cx92Tb_-B7ZO0EZZwwUBhAao';
const OKTA_SCOPE = __ENV.OKTA_SCOPE || 'openid';

const OKTA_USERNAME = __ENV.OKTA_USERNAME || 'test1@ac.com';
const OKTA_PASSWORD = __ENV.OKTA_PASSWORD || 'P@ss0d1234';

let ACCESS_TOKEN = null;
let ACCESS_TOKEN_EXP = null;

export function getOktaToken() {

    if (!ACCESS_TOKEN_EXP) return authenticateUsingOkta();

    if (Date.now() >= ACCESS_TOKEN_EXP) { ACCESS_TOKEN = null; }

    if (!ACCESS_TOKEN) return authenticateUsingOkta();
}

function authenticateUsingOkta() {
    const url = `https://${OKTA_DOMAIN}/oauth2/${OKTA_AUTHSERVER_ID}/v1/token`;

    const requestBody = {
        grant_type: 'password',
        client_id: OKTA_CLIENT_ID,
        client_secret: OKTA_CLIENT_SECRET,
        username: OKTA_USERNAME,
        password: OKTA_PASSWORD,
        scope: OKTA_SCOPE
    };

    const response = http.post(url, requestBody);
    const responseJson = response.json();

    ACCESS_TOKEN = responseJson
    ACCESS_TOKEN_EXP = Date.now() + (responseJson.expires_in * 1000);

    return responseJson;
}