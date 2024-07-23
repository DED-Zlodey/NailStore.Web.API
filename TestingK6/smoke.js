import http from 'k6/http';
import { sleep } from 'k6';
import * as config from './config.js';

export const options = {
	vus: 20,
	duration: '30s',
};
export default function () {
	http.get(config.ENDPOINT_CAT_ID_1, {
		headers: {
			accept: 'application/json',
			autorization: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6Ijk2YTc4ZWQwLWI1OTItNDk0My04ODk5LTg5Njk0ZDBiYmVjNyIsIlJvbGUiOiJBZG1pbiIsIm5iZiI6MTcyMTY2NTI3NywiZXhwIjoxNzIxNzUxNjc3LCJpc3MiOiJOYWlsU3RvcmVBcGkiLCJhdWQiOiJOYWlsU3RvcmUuQ29tcGFueSJ9.vcR52hX8WWw_CL55Ycmsa0tjHboU_vW6OHsV5ryji9c',
		},
	});
	http.get(config.ENDPOINT_CAT_ID_2, {
		headers: {
			accept: 'application/json',
			autorization: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6Ijk2YTc4ZWQwLWI1OTItNDk0My04ODk5LTg5Njk0ZDBiYmVjNyIsIlJvbGUiOiJBZG1pbiIsIm5iZiI6MTcyMTY2NTI3NywiZXhwIjoxNzIxNzUxNjc3LCJpc3MiOiJOYWlsU3RvcmVBcGkiLCJhdWQiOiJOYWlsU3RvcmUuQ29tcGFueSJ9.vcR52hX8WWw_CL55Ycmsa0tjHboU_vW6OHsV5ryji9c',
		},
	});
	sleep(1);
}
