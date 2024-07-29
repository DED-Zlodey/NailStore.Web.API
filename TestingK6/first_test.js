import http from 'k6/http';
import { sleep } from 'k6';
import * as config from './config.js';

export const options = {
	vus: 1,
	duration: '30s',
};
export default function () {
	http.get(config.ENDPOINT_CAT_ID_2);
	sleep(1);
}
