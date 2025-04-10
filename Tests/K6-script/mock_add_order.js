import http from 'k6/http';
import {check, sleep} from 'k6';

export let options = {
    vus: 10,  // 10 Virtual Users
    duration: '320s',  // Run test for 30 seconds
};

export default function () {
    let url = 'http://localhost:9002/api/Order/PlaceOrder'; // Adjust for your API
    let payload = JSON.stringify({
        customerId: __VU,
        productId: __ITER,
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