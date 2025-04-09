import http from 'k6/http';
import {check, sleep} from 'k6';

export let options = {
    vus: 1,  // 10 Virtual Users
    duration: '10s',  // Run test for 30 seconds
};

export default function () {
    let url = 'http://localhost:5283/api/Order/PlaceOrder'; // Adjust for your API
    let payload = JSON.stringify({
        customerId: Math.floor(Math.random() * 100) + 1,
        productId: Math.floor(Math.random() * 100) + 1,
        quantity: Math.floor(Math.random() * 100) + 1
    });

    let params = {
        headers: {'Content-Type': 'application/json'},
    };

    let res = http.post(url, payload, params);

    check(res, {
        'is status 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1);
}