from locust import FastHttpUser, task, constant


class SasBackendUser(FastHttpUser):
    wait_time = constant(0)

    @task
    def get_instance(self):
        self.client.get(
            "/health/instance",
            name="/health/instance"
        )
