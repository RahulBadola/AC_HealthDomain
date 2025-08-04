import { describe, expect } from 'https://jslib.k6.io/k6chaijs/4.3.4.1/index.js';
import http from 'k6/http';

export const BASE_URL = __ENV.API_BASEURL
    || 'https://coreprod.assurecaremc.com/core-prod-decomdev/type-lookup'

const endpointName = {
    AuthorizationRoleOperations: 'api/authorization/{securityRoleId}/operations',
    AuthorizationRoleOperationsTyped: 'api/authorization/{securityRoleId}/operations/{operationType}',
    AuthorizationGetOperationsByRole: 'api/authorization/{securityRoleId}/operations/all',
    AuthorizationGetOperationsByObject: 'api/authorization/{objectName}/operations',
    AuthorizationOperationsQuery: 'api/authorization/operations/query',

    GenericLookup: 'api/genericlookup',

    TypeLookupGetMany: 'api/typelookup',
}

function authorizationGetRoleOperations(h, query) {
    let response;

    describe("AuthorizationController-GetRoleOperations", function () {
        const params = Object.assign({ tags: { name: endpointName.AuthorizationOperationsQuery } }, h);
        response = http.post(`${BASE_URL}/api/authorization/operations/query`, JSON.stringify(query), params);

        writeErrorToConsole(response);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function authorizationGetAllowedOperations(h, securityRoleId) {
    let response;

    describe("AuthorizationController-GetAllowedOperations", function () {
        const params = Object.assign({ tags: { name: endpointName.AuthorizationRoleOperations } }, h);
        response = http.get(`${BASE_URL}/api/authorization/${securityRoleId}/operations`, params);

        writeErrorToConsole(response);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function authorizationGetAllowedOperationsOfType(h, securityRoleId, operationType) {
    let response;

    describe("AuthorizationController-GetAllowedOperationsOfType", function () {
        const params = Object.assign({ tags: { name: endpointName.AuthorizationRoleOperationsTyped } }, h);
        response = http.get(`${BASE_URL}/api/authorization/${securityRoleId}/operations/${operationType}`, params);

        writeErrorToConsole(response);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function authorizationGetAllOperationsBySecurityRole(h, securityRoleId) {
    let response;

    describe("AuthorizationController-GetAllOperationsBySecurityRole", function () {
        const params = Object.assign({ tags: { name: endpointName.AuthorizationGetOperationsByRole } }, h);
        response = http.get(`${BASE_URL}/api/authorization/${securityRoleId}/operations/all`, params);

        writeErrorToConsole(response);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function authorizationGetOperationsByObject(h, objectName) {
    let response;

    describe("AuthorizationController-GetOperationsByObject", function () {
        const params = Object.assign({ tags: { name: endpointName.AuthorizationGetOperationsByObject } }, h);
        response = http.get(`${BASE_URL}/api/authorization/${objectName}/operations`, params);

        writeErrorToConsole(response);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function genericLookupGetMany(h, typeNames) {
    let response;

    describe("GenericLookupController-GetMany", function () {
        const params = Object.assign({ tags: { name: endpointName.GenericLookup } }, h);
        params.headers['typeNames'] = typeNames.join(',');
        response = http.get(`${BASE_URL}/api/genericlookup`, params);

        writeErrorToConsole(response);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function typeLookupGetMany(h, typeNames) {
    let response;

    describe("TypeLookupController-GetMany", function () {
        const params = Object.assign({ tags: { name: endpointName.TypeLookupGetMany } }, h);
        params.headers['typeNames'] = typeNames.join(',');
        response = http.get(`${BASE_URL}/api/typelookup`, params);

        writeErrorToConsole(response);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function writeErrorToConsole(response) {
    if (response.status != 200) {
        console.error(`HTTP ${response.request.method} ${response.request.url}\n  status: ${response.status_text}\n  body: ${response.body}`);
    }
}

export const typeLookupService = {
    authorization: {
        getAllowedOperations: authorizationGetAllowedOperations,
        getAllowedOperationsOfType: authorizationGetAllowedOperationsOfType,
        getAllOperationsBySecurityRole: authorizationGetAllOperationsBySecurityRole,
        getOperationsByObject: authorizationGetOperationsByObject,
        operationsQuery: authorizationGetRoleOperations,
    },
    genericLookup: {
        getMany: genericLookupGetMany,
    },
    typeLookup: {
        getMany: typeLookupGetMany,
    },
};
