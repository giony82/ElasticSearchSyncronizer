import time
from locust import HttpUser, task, between
from datetime import date

class QuickstartUser(HttpUser):
    wait_time = between(1, 2)    

    @task(1)
    def view_item(self):
        for item_id in range(10):
            today = date.today()
            self.client.post(f"student/increment/5dc01d76-1c01-4e3d-12a3-08d99e03611b/1", json={})
            time.sleep(1)    