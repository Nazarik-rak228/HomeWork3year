package org.example.creatfirtsapi.repository;


import org.example.creatfirtsapi.dto.MessageDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface MessageRepository extends JpaRepository<MessageDto, Integer> {
    List<MessageDto> findBySenderId(int senderId);
}
/*

@Repository
public class MessageRepository {
    private List<MessageModel> messages = new ArrayList<>();
    private AtomicInteger idCounter = new AtomicInteger(1);

    public List<MessageModel> findAll() {
        return new ArrayList<>(messages);
    }

    public MessageModel addMessage(MessageModel message) {
        message.setId(idCounter.getAndIncrement());
        messages.add(message);
        return message;
    }

    public MessageModel updateMessage(MessageModel message) {
        for (int i = 0; i < messages.size(); i++) {
            if (messages.get(i).getId() == message.getId()) {
                messages.set(i, message);
                return message;
            }
        }
        return null;
    }

    public void deleteMessage(int id) {
        messages.removeIf(message -> message.getId() == id);
    }

    public MessageModel findById(int id) {
        return messages.stream()
                .filter(message -> message.getId() == id)
                .findFirst()
                .orElse(null);
    }
    public List<MessageModel> findBySenderId(int senderId) {
        return messages.stream()
                .filter(m -> m.getSenderId() == senderId)
                .toList();
    }

}
*/
