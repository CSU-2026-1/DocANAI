import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '10s', target: 2000 },
        { duration: '20s', target: 2000 },
        { duration: '10s', target: 0 },
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], 
    },
};

export function setup() {
    const authUrl = 'http://localhost:8080/api/v1/auth';
    const uniqueUsername = `k6_user_${Math.floor(Math.random() * 1000000)}`;
    const password = 'TemporaryPassword123!';

    const headers = { 'Content-Type': 'application/json' };
    
    http.post(`${authUrl}/register`, JSON.stringify({
        username: uniqueUsername,
        password: password
    }), { headers });
    
    const loginRes = http.post(`${authUrl}/login`, JSON.stringify({
        username: uniqueUsername,
        password: password
    }), { headers });

    let token = '';
    try {
        const body = JSON.parse(loginRes.body);
        token = body.accessToken;
    } catch (e) {
        console.error('Не удалось получить токен при входе: ' + loginRes.body);
    }
    
    return { token: token };
}

export default function (data) {
    const url = 'http://localhost:8080/api/v1/ai-models?includeUnavailable=false';

    const params = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${data.token}`,
        },
    };

    const res = http.get(url, params);
    
    check(res, {
        'status is 200': (r) => r.status === 200,
        'transaction time OK': (r) => r.timings.duration < 500,
    });

    sleep(1);
}