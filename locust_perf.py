import time
from locust import HttpUser, task, between
from datetime import date

class QuickstartUser(HttpUser):
    wait_time = between(1, 2)    

    @task(1)
    def view_item(self):
        for item_id in range(10):
            today = date.today()
            self.client.post(f"student", json={"name":f"some name {item_id} {today}"})
            time.sleep(1)    