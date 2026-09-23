package org.example.creatfirtsapi.repository;


import org.example.creatfirtsapi.dto.ResponseDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ResponseRepository extends JpaRepository<ResponseDto, Integer> {
    List<ResponseDto> findByStatusContainingIgnoreCase(String status);
    List<ResponseDto> findByTask_Id(int taskId);
}
/*

@Repository
public class ResponseRepository {

    private List<ResponseModel> responses = new ArrayList<>();
    private AtomicInteger idCounter = new AtomicInteger(1);

    public List<ResponseModel> findAll() {
        return new ArrayList<>(responses);
    }

    public ResponseModel addResponse(ResponseModel response) {
        response.setId(idCounter.getAndIncrement());
        responses.add(response);
        return response;
    }

    public ResponseModel updateResponse(ResponseModel response) {
        for (int i = 0; i < responses.size(); i++) {
            if (responses.get(i).getId() == response.getId()) {
                responses.set(i, response);
                return response;
            }
        }
        return null;
    }

    public void deleteResponse(int id) {
        responses.removeIf(response -> response.getId() == id);
    }

    public ResponseModel findById(int id) {
        return responses.stream()
                .filter(response -> response.getId() == id)
                .findFirst()
                .orElse(null);
    }
    public List<ResponseModel> findByStatus(String status) {
        return responses.stream()
                .filter(r -> r.getStatus() != null &&
                        r.getStatus().toLowerCase().contains(status.toLowerCase()))
                .toList();
    }

    public List<ResponseModel> findByTaskId(int taskId) {
        return responses.stream()
                .filter(r -> r.getTaskId() == taskId)
                .toList();
    }
}*/
