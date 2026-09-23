package org.example.creatfirtsapi.repository;

import org.example.creatfirtsapi.dto.UserDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface UserRepository extends JpaRepository<UserDto, Integer> {
    List<UserDto> findByUsernameContainingIgnoreCase(String username);
    Optional<UserDto> findByEmail(String email);
    Optional<UserDto> findFirstByUsername(String username);
}

/*@Repository
public class UserRepository {

    private List<UserModel> users = new ArrayList<>();
    private AtomicInteger idCounter = new AtomicInteger(1);

    public List<UserModel> findAll() {
        return new ArrayList<>(users);
    }

    public UserModel addUser(UserModel user) {
        user.setId(idCounter.getAndIncrement());
        users.add(user);
        return user;
    }

    public UserModel updateUser(UserModel user) {
        for (int i = 0; i < users.size(); i++) {
            if (users.get(i).getId() == user.getId()) {
                users.set(i, user);
                return user;
            }
        }
        return null;
    }

    public void deleteUser(int id) {
        users.removeIf(user -> user.getId() == id);
    }

    public UserModel findById(int id) {
        return users.stream()
                .filter(user -> user.getId() == id)
                .findFirst()
                .orElse(null);
    }
    public List<UserModel> findByUsername(String username) {
        return users.stream()
                .filter(u -> u.getUsername() != null &&
                        u.getUsername().toLowerCase().contains(username.toLowerCase()))
                .toList();
    }

    public List<UserModel> findByEmail(String email) {
        return users.stream()
                .filter(u -> u.getEmail() != null &&
                        u.getEmail().toLowerCase().contains(email.toLowerCase()))
                .toList();
    }

}*/