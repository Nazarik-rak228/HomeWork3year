package org.example.creatfirtsapi.repository;


import org.example.creatfirtsapi.dto.UserTaskDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface UserTaskRepository extends JpaRepository<UserTaskDto, Integer> {
    List<UserTaskDto> findByStatusContainingIgnoreCase(String status);
    List<UserTaskDto> findByUserId(int userId);
}
/*
@Repository

public class UserTaskRepository {
    private List<UserTaskModel> userTasks = new ArrayList<>();
    private AtomicInteger idCounter = new AtomicInteger(1);

    public List<UserTaskModel> findAll() {
        return new ArrayList<>(userTasks);
    }

    public UserTaskModel addUserTask(UserTaskModel userTask) {
        userTask.setId(idCounter.getAndIncrement());
        userTasks.add(userTask);
        return userTask;
    }

    public UserTaskModel updateUserTask(UserTaskModel userTask) {
        for (int i = 0; i < userTasks.size(); i++) {
            if (userTasks.get(i).getId() == userTask.getId()) {
                userTasks.set(i, userTask);
                return userTask;
            }
        }
        return null;
    }

    public void deleteUserTask(int id) {
        userTasks.removeIf(userTask -> userTask.getId() == id);
    }

    public UserTaskModel findById(int id) {
        return userTasks.stream()
                .filter(userTask -> userTask.getId() == id)
                .findFirst()
                .orElse(null);
    }
    public List<UserTaskModel> findByStatus(String status) {
        return userTasks.stream()
                .filter(ut -> ut.getStatus() != null &&
                        ut.getStatus().toLowerCase().contains(status.toLowerCase()))
                .toList();
    }

    public List<UserTaskModel> findByUserId(int userId) {
        return userTasks.stream()
                .filter(ut -> ut.getUserId() == userId)
                .toList();
    }}
*/
