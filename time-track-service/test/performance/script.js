import { SharedArray } from 'k6/data';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';
import { jUnit, textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js';
import { getOktaToken } from './okta.js';
import { BASE_URL, timeTrackService } from './time-track-services.js'

const TENANT_ID = 'd5a28601-2b37-41dc-abd6-2b0d940c7938';
const SEGMENT_ID = '00000000-0000-0000-0000-000000000000';
const USER_ROLE_ID = __ENV.USER_ROLE_ID  || '00000000-0000-0000-0000-00000000000d';

export const options = {
    thresholds: {
        http_req_failed: ['rate<0.01'],

        'group_duration{group:::createStaffTimeTrack}': ['avg < 30000'],
        'group_duration{group:::getStaffTimeTracks}': ['avg < 30000'],
        'group_duration{group:::getStaffTimeTrack}': ['avg < 30000'],
        'group_duration{group:::updateStaffTimeTrack}': ['avg < 30000'],
        'group_duration{group:::voidStaffTimeTrack}': ['avg < 30000'],

        'group_duration{group:::createMemberTimeTrack}': ['avg < 30000'],
        'group_duration{group:::getMemberTimeTracks}': ['avg < 30000'],
        'group_duration{group:::getMemberTimeTrack}': ['avg < 30000'],
        'group_duration{group:::updateMemberTimeTrack}': ['avg < 30000'],
        'group_duration{group:::voidMemberTimeTrack}': ['avg < 30000'],

        'http_req_duration{method: GET, name:/api/staff/{staffGuid}/timetracking}': ['avg < 2000'],
        'http_req_duration{method: POST, name:/api/staff/{staffGuid}/timetracking}': ['avg < 2000'],
        'http_req_duration{method: GET, name:/api/staff/{staffGuid}/timetracking/{id}}': ['avg < 2000'],

        'http_req_duration{method: POST, name:/api/members/{memberGuid}/timetracking}': ['avg < 2000'],
        'http_req_duration{method: GET, name:/api/members/{memberGuid}/timetracking}': ['avg < 2000'],
        'http_req_duration{method: GET, name:/api/members/{memberGuid}/timetracking/{id}}': ['avg < 2000'],
    }
};



const shared = {
    members: new SharedArray('members', function () {
        return JSON.parse(open('./data/members.json'))
    }),
    staffs: new SharedArray('staffs', function () {
        return JSON.parse(open('./data/staffs.json'))
    })
}

export function setup() {
    console.log(`BASE_URL: ${BASE_URL}`)

    return {
        okta: getOktaToken()
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
            'SecurityUserRoleId': USER_ROLE_ID,
        }
    };

    const memberId = shared.members[Math.floor(Math.random() * shared.members.length)].MemberId;
	const staffId = shared.staffs[Math.floor(Math.random() * shared.staffs.length)].staffId;

    timeTrackService.getStaffTimeTracks(h, staffId);
    let staffResponse = timeTrackService.createStaffTimeTrack(h, staffId);
    const staffResponseId = staffResponse.json('Id');
    staffResponse = timeTrackService.updateStaffTimeTrack(h, staffResponse.json(), staffId);
	staffResponse = timeTrackService.getStaffTimeTrack(h, staffId, staffResponseId);
    timeTrackService.voidStaffTimeTrack(h, staffResponse.json(), staffId);

    timeTrackService.getMemberTimeTracks(h, memberId);
    let memberResponse =timeTrackService.createMemberTimeTrack(h, memberId);
    const memberResponseId = memberResponse.json('Id');
    memberResponse = timeTrackService.updateMemberTimeTrack(h, memberResponse.json(), memberId);
    memberResponse = timeTrackService.getMemberTimeTrack(h, memberId, memberResponseId);
    timeTrackService.voidMemberTimeTrack(h, memberResponse.json(), memberId);
}

export function handleSummary(data) {
    const STAGE_NAME = __ENV.SYSTEM_STAGEDISPLAYNAME
    const JOB_NAME = __ENV.SYSTEM_JOBDISPLAYNAME

    let name = STAGE_NAME;
    if (JOB_NAME) {
        name += ` - ${JOB_NAME}`
    }

    return {
        'stdout': textSummary(data, { indent: '  ', enableColors: true }),
        './TEST-PERFORMANCE-RESULTS.html': htmlReport(data),
        './TEST-PERFORMANCE-RESULTS.json': JSON.stringify(data),
        './TEST-PERFORMANCE-RESULTS.txt': textSummary(data, { enableColors: false }),
        './TEST-PERFORMANCE-RESULTS.xml': jUnit(data, { name })
    }
}