import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 100,  // 10 Virtual Users
    duration: '120s',  // Run test for 30 seconds
};

export default function () {
    let url = 'http://localhost:9002/api/Product/AddProduct'; // Adjust for your API
    let payload = JSON.stringify({
        name: `Mock Product ${__VU}-${__ITER}`,
        price: Math.random() * 100,
        quantity: Math.floor(Math.random() * 50) + 1
    });

    let params = {
        headers: { 'Content-Type': 'application/json' },
    };

    let res = http.post(url, payload, params);

    check(res, {
        'is status 201': (r) => r.status === 201,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    // sleep(1);
}