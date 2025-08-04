import { describe, expect } from 'https://jslib.k6.io/k6chaijs/4.3.4.1/index.js';
import http from 'k6/http';

export const BASE_URL = __ENV.API_BASEURL
    || 'https://dtcentralus.assurecaremc.com/core-prod-dev2/time-track'
    || 'https://dtcentralus.assurecaremc.com/core-prod-dev2/smart-routing-proxy'
    || 'https://dev-02.assurecaremc.com'

const endpointName = {
    StaffTimeTrack: '/api/staff/{staffGuid}/timetracking',
    StaffTimeTrackId: '/api/staff/{staffGuid}/timetracking/{id}',
	MemberTimeTrack: '/api/members/{memberGuid}/timetracking',
    MemberTimeTrackId: '/api/members/{memberGuid}/timetracking/{id}',
}

function createStaffTimeTrack(h, staffGuid) {
	 if (!staffGuid) return null;

	const staffTimeTrack = {
        "Id": null,
        "Case": null,
        "CaseList": null,
        "MemberProgramGuid": null,
        "StartDate": "2022-09-01T14:53:57.575+05:30",
        "EndDate": "2022-09-01T14:53:57.575+05:30",
        "StartTime": "14:53:57 +05:30",
        "EndTime": "14:53:57 +05:30",
        "FundingSource": null,
        "MemberId": null,
        "Member": null,
        "ProgramType": null,
        "SecurityUserId": null,
        "Service": null,
        "ServiceUnits": null,
        "Activity": "ASMT",
        "SubActivity": null,
        "TotalTime": 0,
        "TotalTimeDisplay": "00:00",
        "TravelDuration": null,
        "TravelMiles": null,
        "VolunteerDriver": null,
        "Reason": null,
        "AdditionalComments": null,
        "InsertedByName": null,
        "InsertedOn": null,
        "UpdatedByName": null,
        "UpdatedOn": null,
        "Udf": {},
        "CareProgramRequired": null
    }

    let response;

    describe("createStaffTimeTrack", function () {
        const params = Object.assign({ tags: { name: endpointName.StaffTimeTrack } }, h);
        response = http.post(`${BASE_URL}/api/staff/${staffGuid}/timetracking`, JSON.stringify(staffTimeTrack), params);

        if (response.status != 200) {
            console.error(`createStaffTimeTrack error: ${response.error_code} - ${response.error}`);
            console.log(`createStaffTimeTrack response: ${response.body}`);
        }

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });
    return response;
}

function getStaffTimeTrack(h, staffGuid, id) {
    if (!id) return null;

    let response;

    describe("getStaffTimeTrack", function () {
        const params = Object.assign({ tags: { name: endpointName.StaffTimeTrackId } }, h);
        response = http.get(`${BASE_URL}/api/staff/${staffGuid}/timetracking/${id}`, params);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });
    return response;
}

function getStaffTimeTracks(h, staffGuid) {
     if (!staffGuid) return null;

    let response;

    describe("getStaffTimeTracks", function () {
        const params = Object.assign({ tags: { name: endpointName.StaffTimeTrack } }, h);
        response = http.get(`${BASE_URL}/api/staff/${staffGuid}/timetracking`, params);

       expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });
    return response;
}

function updateStaffTimeTrack(h, staff, staffGuid) {
    if (!staff) return null;

    staff.UpdatedOn = new Date().toISOString().slice(0, 10); // today
    staff.AdditionalComments = "test1";

    let response;

    describe("updateStaffTimeTrack", function () {
        const params = Object.assign({ tags: { name: endpointName.StaffTimeTrack } }, h);
        response = http.post(`${BASE_URL}/api/staff/${staffGuid}/timetracking`, JSON.stringify(staff), params);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function voidStaffTimeTrack(h, staff, staffGuid) {
    if (!staff) return null;

    staff.UpdatedOn = new Date().toISOString().slice(0, 10); // today
    staff['VoidReasonId'] = '1c5ddf10-cd75-41d4-b099-514c915e6369';
    let response;

    describe("voidStaffTimeTrack", function () {
        const params = Object.assign({ tags: { name: endpointName.StaffTimeTrack } }, h);
        response = http.post(`${BASE_URL}/api/staff/${staffGuid}/timetracking`, JSON.stringify(staff), params);
        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function createMemberTimeTrack(h, memberGuid) {
    if (!memberGuid) return null;

   const timeTrack = {
            "Id": null,
            "Case": null,
            "CaseList": [
                {
                    "Case": "Gilen - Open (tbd-tbd)",
                    "MemberProgramId": "27a0946d-cca6-452d-91f3-9c4da6cb3382",
                    "ProgramTypeKey": "Gilen"
                },
                {
                    "Case": "Algorithms - Closed (tbd-8/8/2022)",
                    "MemberProgramId": "1c9101c6-d806-43fa-affd-c30b0e1dc97d",
                    "ProgramTypeKey": "Algorithms"
                },
                {
                    "Case": "DSNP - Closed (tbd-8/8/2022)",
                    "MemberProgramId": "d4cd66c9-ddda-4311-801a-1f93de90a2a9",
                    "ProgramTypeKey": "DSNP"
                }
            ],
            "MemberProgramGuid": null,
            "StartDate": "2022-09-02T10:21:36.880+05:30",
            "EndDate": "2022-09-02T10:21:36.880+05:30",
            "StartTime": "10:21:36 +05:30",
            "EndTime": "10:21:36 +05:30",
            "FundingSource": null,
            "MemberId": "8d80848e-8914-4afb-955a-0e287693348a",
            "Member": "sudheer Gottumukkala",
            "ProgramType": null,
            "SecurityUserId": null,
            "Service": null,
            "ServiceUnits": null,
            "Activity": "CM",
            "SubActivity": null,
            "TotalTime": 0,
            "TotalTimeDisplay": "00:00",
            "TravelDuration": null,
            "TravelMiles": null,
            "VolunteerDriver": null,
            "Reason": null,
            "AdditionalComments": null,
            "InsertedByName": null,
            "InsertedOn": null,
            "UpdatedByName": null,
            "UpdatedOn": null,
            "Udf": {},
            "CareProgramRequired": null
        }

   let response;

   describe("createMemberTimeTrack", function () {
       const params = Object.assign({ tags: { name: endpointName.MemberTimeTrack } }, h);
       response = http.post(`${BASE_URL}/api/members/${memberGuid}/timetracking`, JSON.stringify(timeTrack), params);

       if (response.status != 200) {
           console.error(`createMemberTimeTrack error: ${response.error_code} - ${response.error}`);
           console.log(`createMemberTimeTrack response: ${response.body}`);
       }

       expect(response.status, 'http status code').to.equal(200);
       expect(response).to.have.validJsonBody();
   });
   return response;
}

function getMemberTimeTrack(h, memberGuid, id) {
    if (!id) return null;

    let response;

    describe("getMemberTimeTrack", function () {
        const params = Object.assign({ tags: { name: endpointName.MemberTimeTrackId } }, h);
        response = http.get(`${BASE_URL}/api/members/${memberGuid}/timetracking/${id}`, params);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });
    return response;
}

function getMemberTimeTracks(h, memberGuid) {
     if (!memberGuid) return null;

    let response;

    describe("getMemberTimeTracks", function () {
        const params = Object.assign({ tags: { name: endpointName.MemberTimeTrack } }, h);
        response = http.get(`${BASE_URL}/api/members/${memberGuid}/timetracking`, params);

       expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });
    return response;
}

function updateMemberTimeTrack(h, memberData, memberGuid) {
    if (!memberData) return null;

    memberData.UpdatedOn = new Date().toISOString().slice(0, 10); // today
    memberData.AdditionalComments = "test1";

    let response;

    describe("updateMemberTimeTrack", function () {
        const params = Object.assign({ tags: { name: endpointName.MemberTimeTrack } }, h);
        response = http.post(`${BASE_URL}/api/members/${memberGuid}/timetracking`, JSON.stringify(memberData), params);

        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

function voidMemberTimeTrack(h, memberData, memberGuid) {
    if (!memberData) return null;

    memberData.UpdatedOn = new Date().toISOString().slice(0, 10); // today
    memberData['VoidReasonId'] = '1c5ddf10-cd75-41d4-b099-514c915e6369';
    let response;

    describe("voidMemberTimeTrack", function () {
        const params = Object.assign({ tags: { name: endpointName.MemberTimeTrack } }, h);
        response = http.post(`${BASE_URL}/api/members/${memberGuid}/timetracking`, JSON.stringify(memberData), params);
        expect(response.status, 'http status code').to.equal(200);
        expect(response).to.have.validJsonBody();
    });

    return response;
}

export const timeTrackService = {
	createStaffTimeTrack,
	getStaffTimeTrack,
	getStaffTimeTracks,
    updateStaffTimeTrack,
    voidStaffTimeTrack,
    createMemberTimeTrack,
    getMemberTimeTrack,
    getMemberTimeTracks,
    updateMemberTimeTrack,
    voidMemberTimeTrack
}