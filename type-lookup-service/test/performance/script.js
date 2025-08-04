import { SharedArray } from 'k6/data';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';
import { jUnit, textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js';
import { getOktaToken } from './okta.js';
import { BASE_URL } from './type-lookup.js';
import { typeLookupService } from './type-lookup.js';
import { getSingleRandomElement } from "./utils.js";

const TENANT_ID = 'd5a28601-2b37-41dc-abd6-2b0d940c7938';
const SEGMENT_ID = '00000000-0000-0000-0000-000000000000';
const USER_ROLE_ID = __ENV.USER_ROLE_ID || '00000000-0000-0000-0000-00000000000d';

export const options = {
    thresholds: {
        http_req_failed: ['rate<0.01'],

        'group_duration{group:::AuthorizationController-GetRoleOperations}': ['avg < 30000'],
        'http_req_duration{method: POST, name: api/authorization/operations/query}': ['avg < 3000'],

        'group_duration{group:::AuthorizationController-GetAllowedOperations}': ['avg < 30000'],
        'http_req_duration{method: GET, name: api/authorization/{securityRoleId}/operations}': ['avg < 3000'],

        'group_duration{group:::AuthorizationController-GetAllowedOperationsTyped}': ['avg < 30000'],
        'http_req_duration{method: GET, name: api/authorization/{securityRoleId}/operations/{operationType}}': ['avg < 3000'],

        'group_duration{group:::AuthorizationController-GetAllOperationsBySecurityRole}': ['avg < 30000'],
        'http_req_duration{method: GET, name: api/authorization/{securityRoleId}/operations/all}': ['avg < 3000'],

        'group_duration{group:::AuthorizationController-GetOperationsByObject}': ['avg < 30000'],
        'http_req_duration{method: GET, name: api/authorization/{objectName}/operations}': ['avg < 3000'],

        'group_duration{group:::GenericLookupController-GetMany}': ['avg < 30000'],
        'http_req_duration{method: GET, name: api/genericlookup}': ['avg < 3000'],

        'group_duration{group:::TypeLookupController-GetMany}': ['avg < 30000'],
        'http_req_duration{method: GET, name: api/typelookup}': ['avg < 3000'],
    },
};

const shared = {
    objectNames: new SharedArray("objectNames", function () {
        return JSON.parse(open('./data/objectNames.json'));
    }),
    operationTypes: new SharedArray("operationTypes", function () {
        return JSON.parse(open('./data/operationTypes.json'));
    }),
    typeNames: new SharedArray("typeNames", function () {
        return JSON.parse(open('./data/typeNames.json'));
    }),

}

export function setup() {
    console.log(`BASE_URL: ${BASE_URL}`);

    return {
        okta: getOktaToken(),
    };
}

export default function (data) {
    const h = {
        headers: {
            'User-Agent': 'k6',
            'Authorization': `Bearer ${data.okta.access_token}`,
            'Content-Type': 'application/json',
            'accept': 'application/json',
            'TenantId': TENANT_ID,
            'SegmentId': SEGMENT_ID,
            'SecurityUserRoleId': USER_ROLE_ID
        }
    };

    typeLookupService.authorization.operationsQuery(h, {
        ObjectNames: shared.objectNames,
        SecurityRoleIds: [USER_ROLE_ID],
    });

    typeLookupService.authorization.getAllowedOperations(h, USER_ROLE_ID);

    const operationType = getSingleRandomElement(shared.operationTypes);
    typeLookupService.authorization.getAllowedOperationsOfType(h, USER_ROLE_ID, operationType);

    typeLookupService.authorization.getAllOperationsBySecurityRole(h, USER_ROLE_ID);

    const objectName = getSingleRandomElement(shared.objectNames);
    typeLookupService.authorization.getOperationsByObject(h, objectName);

    const genericLookupTypeNames = [getSingleRandomElement(shared.typeNames),];
    typeLookupService.genericLookup.getMany(h, genericLookupTypeNames);

    const typeLookupTypeNames = [getSingleRandomElement(shared.typeNames),];
    typeLookupService.typeLookup.getMany(h, typeLookupTypeNames);
}

export function handleSummary(data) {
    const STAGE_NAME = __ENV.SYSTEM_STAGEDISPLAYNAME;
    const JOB_NAME = __ENV.SYSTEM_JOBDISPLAYNAME;

    let name = STAGE_NAME;
    if (JOB_NAME) {
        name += ` - ${JOB_NAME}`;
    }

    return {
        'stdout': textSummary(data, { indent: '  ', enableColors: true }),
        './TEST-PERFORMANCE-RESULTS.html': htmlReport(data),
        './TEST-PERFORMANCE-RESULTS.json': JSON.stringify(data),
        './TEST-PERFORMANCE-RESULTS.txt': textSummary(data, { enableColors: false }),
        './TEST-PERFORMANCE-RESULTS.xml': jUnit(data, { name }),
    }
}
